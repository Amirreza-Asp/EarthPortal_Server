using Application;
using Application.Contracts.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserAccessor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUserName()
        {
            return _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(b => b.Type == AppClaims.UserName)?.Value ?? string.Empty;
        }
    }
}
