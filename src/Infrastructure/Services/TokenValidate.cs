using Application.Contracts.Infrastructure.Services;
using Application.Utilities;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
            var authInfoJsonDecript = context.Request.Cookies[SD.AuthToken];

            if (String.IsNullOrEmpty(authInfoJsonDecript))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }


            var token = ProtectorData.Decrypt(authInfoJsonDecript);

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            var username = JWTokenService.GetTokenUserName(claimsIdentity);

            String userCacheKey = $"user-{username}";
            var user = new Domain.Entities.Account.User();

            if (!_memoryCache.TryGetValue(userCacheKey, out user))
            {
                user = await _context.User
                    .Include(b => b.Role)
                    .FirstOrDefaultAsync(x => x.UserName == username);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(userCacheKey, user, cacheEntryOptions);
            }

            if (user == null || user.IsActive == false)
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }


            if (JWTokenService.GetTokenExpirationTime(claimsIdentity) <= DateTime.UtcNow)
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
