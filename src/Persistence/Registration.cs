using Application.Contracts.Persistence.Repositories;
using Application.Contracts.Persistence.Services;
using Application.Contracts.Persistence.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Services;
using Persistence.Utilities;
using System.Reflection;

namespace Persistence
{
    public static class Registration
    {

        public static IServiceCollection AddPersistencsRegistrations(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                //options.UseInMemoryDatabase("Earth");
            });

            // Initializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Services
            services.AddScoped<IIranelandService, IranelandService>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ILawRepository, LawRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<ISystemEvaluationRepository, SystemEvaluationRepository>();
            services.AddScoped<IHomePageRepository, HomePageRepository>();
            services.AddScoped<IEnglishPageRepository, EnglishPageRepository>();
            services.AddScoped<IUserCounterService, UserCounterService>();

            return services;
        }

    }
}
