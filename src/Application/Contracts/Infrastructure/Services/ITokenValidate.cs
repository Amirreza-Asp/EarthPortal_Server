using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Application.Contracts.Infrastructure.Services
{
    public interface ITokenValidate
    {
        Task Execute(TokenValidatedContext context);
    }
}
