using Application.Contracts.Infrastructure.Services;
using Application.Utilities;
using Domain;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Persistence;
using System.Security.Claims;

namespace Infrastructure.Services
{
    internal class TokenValidate : ITokenValidate
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public TokenValidate(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task Execute(TokenValidatedContext context)
        {
            var encryptedAuthInfoJson = context.Request.Cookies[SD.AuthToken];

            if (String.IsNullOrEmpty(encryptedAuthInfoJson))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }


            var token = ProtectorData.Decrypt(encryptedAuthInfoJson);

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            var username = JWTokenService.GetTokenUserName(claimsIdentity);

            String userCacheKey = $"user-{username}";

            if (!_memoryCache.TryGetValue(userCacheKey, out User user))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            if (user == null || user.IsActive == false)
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            if (JWTokenService.GetTokenIpAddress(claimsIdentity) != context.Request.HttpContext.Connection.RemoteIpAddress.ToString())
            {
                context.Fail("ip شما نسبت به زمان احراز هویت تغییر کرده است و نشست شما نامعتبر می باشد");
                return;
            }

            var role = JWTokenService.GetTokenRole(claimsIdentity);
            if (string.IsNullOrEmpty(role))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }
        }
    }
}
