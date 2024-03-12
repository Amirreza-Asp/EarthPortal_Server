using Application;
using Application.Contracts.Persistence.Utilities;
using Infrastructure;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
            .WithOrigins("http://localhost:4173/", "http://localhost:5173", "http://172.16.0.2:3000", "http://172.33.21.101:3000", "https://localhost:7121", "https://840f-2a09-bac5-41dc-505-00-80-ec.ngrok-free.app/", "http://localhost:5174", "https://earth-portal-client.vercel.app")
            .AllowCredentials();
    });
});


//builder.Services.AddMvc();




var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await initializer.Execute();
});

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();



app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

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

app.MapControllers();

app.Run();
