using Application;
using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Utilities;
using Application.Utilities;
using AspNetCoreRateLimit;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


//var logger = new LoggerConfiguration()
//  .ReadFrom.Configuration(builder.Configuration)
//  .Enrich.FromLogContext()
//  .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);


builder.Services.AddDistributedMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

builder.Services.AddInMemoryRateLimiting();

builder.Services
    .AddApplicationRegistrations()
    .AddPersistencsRegistrations(builder.Configuration)
    .AddInfrastructureRegistrations();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4173", "http://localhost:5173", "https://newportal.iraneland.ir", "http://192.168.142.49:3000", "http://newportal.iraneland.ir", "http://172.33.21.101:3000", "https://localhost:7121", "https://840f-2a09-bac5-41dc-505-00-80-ec.ngrok-free.app/", "http://localhost:5174", "https://earth-portal-client.vercel.app")
            .AllowCredentials();
    });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAntiforgery(o => { o.Cookie.Name = "X-XSRF"; o.HeaderName = "X-XCSRF"; o.SuppressXFrameOptionsHeader = false; });

#region JWT
builder.Services.AddAuthentication(options =>
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
                var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidate>();
                return tokenValidatorService.Execute(context);
            },
            OnMessageReceived = context =>
            {
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

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    initializer.Execute();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
app.Use(async (context, next) =>
{
    if (context.Request.Method.ToLower() != "get")
    {
        if (context.Request.Path.Value.StartsWith("/"))
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Delete("X-CSRF");
            context.Response.Cookies.Append("X-CSRF", tokens.RequestToken,
            new CookieOptions()
            {
                HttpOnly = false,
                Secure = context.Request.Scheme == "https",
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
            });
        }

        #region Security headers

        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");
        context.Response.Headers.Remove("X-Frame-Options");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';base-uri 'self';font-src 'self';form-action 'self';frame-ancestors 'self';img-src 'self' data:;object-src 'none';script-src 'self';script-src-attr 'none';style-src 'self' https: 'unsafe-inline';upgrade-insecure-requests"); context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains perload");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Origin-Agent-Cluster", "?1");
        context.Response.Headers.Append("X-DNS-Prefetch-Control", "off");
        context.Response.Headers.Append("X-Download-Options", "noopen");
        context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        #endregion

    }
    await next();
});



app.MapControllers();

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