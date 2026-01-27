using Application;
using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Services;
using Application.Contracts.Persistence.Utilities;
using Application.Utilities;
using AspNetCoreRateLimit;
using Domain;
using Endpoint.BackgroundServices;
using Endpoint.Filters;
using Endpoint.Middlewares;
using Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddMvc(opt =>
{
    opt.Filters.Add(new RemoveServerInfoFilter());
});

//builder.Logging.ClearProviders();
builder.Services.AddMemoryCache();

builder.Host.UseSerilog(
    (ctx, logConfig) =>
    {
        var columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Add(StandardColumn.LogEvent);

        logConfig
            .ReadFrom.Configuration(ctx.Configuration)
            .WriteTo.MSSqlServer(
                ctx.Configuration.GetConnectionString("DefaultConnection"),
                new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true },
                columnOptions: columnOptions
            );
    }
);

builder.Services.AddDistributedMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(
    builder.Configuration.GetSection("IpRateLimitPolicies")
);
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddHttpContextAccessor();

builder
    .Services.AddApplicationRegistrations()
    .AddPersistencsRegistrations(builder.Configuration)
    .AddInfrastructureRegistrations();

builder.Services.AddHostedService<CasesAndUsersWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(
                    "https://sinic.iraneland.ir",
                    "http://localhost:3000",
                    "https://zamin.gov.ir",
                    "http://localhost:5173",
                    "https://newportal.iraneland.ir"
                )
                .AllowCredentials();
        }
    );
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "MyApp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "clientapp/dist";
});

builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "X-XSRF";

    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.None;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.Path = "/";

    o.HeaderName = "X-XCSRF";
    o.SuppressXFrameOptionsHeader = false;
});

builder.Services.AddSignalR();

#region JWT
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(x =>
    {
        x.Cookie.Name = SD.AuthToken;
    })
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.SaveToken = true;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JWTokenService.Key,
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };
        jwtOptions.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tokenValidatorService =
                    context.HttpContext.RequestServices.GetRequiredService<ITokenValidate>();
                return tokenValidatorService.Execute(context);
            },
            OnMessageReceived = context =>
            {
                var userCounterService =
                    context.HttpContext.RequestServices.GetRequiredService<IUserCounterService>();

                var token = context.Request.Cookies[SD.AuthToken];
                if (token != null)
                {
                    context.Token = ProtectorData.Decrypt(token);
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion

var app = builder.Build();

#region Antiforgery SSL Issue
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedOptions);
#endregion

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    initializer.Execute();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

Log.Information(
    "🚀 Application starting at {Time} on {Machine}",
    DateTime.UtcNow,
    Environment.MachineName
);

app.UseIpRateLimiting();

app.UseRouting();

app.UseCors("CorsPolicy");
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseSpaStaticFiles();

// app.UseStaticFiles();

app.UseCustomHeaderHandler();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
app.Use(
    async (context, next) =>
    {
        if (context.Request.Method.ToLower() != "get")
        {
            if (context.Request.Path.Value.StartsWith("/"))
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Delete("X-CSRF");
                context.Response.Cookies.Append(
                    "X-CSRF",
                    tokens.RequestToken,
                    new CookieOptions()
                    {
                        HttpOnly = false,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                    }
                );
            }
        }

        await next();
    }
);

app.UseEndpoints(endpoints =>
{
    endpoints
        .MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
        .RequireCors("CorsPolicy");
});

app.UseSpa(config =>
{
    config.Options.SourcePath = "clientapp";
    if (app.Environment.IsDevelopment())
    {
        config.UseProxyToSpaDevelopmentServer(new Uri("http://localhost:5173"));
    }
});

app.Run();


























//app.UseSpaStaticFiles();

//app.MapWhen(x => !x.Request.Path.Value.Contains("/api"), builder =>
//{
//    builder.UseSpa(spa =>
//    {
//        //spa.Options.SourcePath = "clientapp";
//        //spa.Options.StartupTimeout = new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 30);


//        //spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
//        //{
//        //    ServeUnknownFileTypes = true, // Ermöglicht das Servieren von Dateien ohne bekannten MIME-Typ
//        //    DefaultContentType = "text/html", // Setzt den Standard-MIME-Typ auf 'text/html'
//        //    OnPrepareResponse = ctx =>
//        //    {

//        //        // Überprüfen, ob die angeforderte Datei die 'index.html' ist
//        //        if (ctx.File.Name.Equals("POST", StringComparison.OrdinalIgnoreCase))
//        //        {
//        //            // Weiterleiten der Anfrage an die Angular-App
//        //            ctx.Context.Request.Path = "/dist/index.html";

//        //            // Führe hier die gewünschten Aktionen aus
//        //            Console.WriteLine("OnPrepareResponse - index.html");
//        //        }
//        //    }
//        //};
//        ////if (app.Environment.IsDevelopment())
//        ////{
//        ////    spa.UseProxyToSpaDevelopmentServer(new Uri("http://localhost:5173"));
//        ////}
//    });
//});
