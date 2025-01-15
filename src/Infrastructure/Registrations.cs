using Application.Contracts.Infrastructure.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Infrastructure.Services.Session;

namespace Infrastructure
{
    public static class Registrations
    {

        public static IServiceCollection AddInfrastructureRegistrations(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IPhotoManager, PhotoManager>();
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<ITokenValidate, TokenValidate>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<ISessionService, SessionService>();

            return services;
        }

    }
}
