using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account;
using Application.Utilities;
using Domain;
using Domain.Entities.Account;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.CQRS.Account
{

    public class SetAuthCookiesNotificationHandler : INotificationHandler<SetAuthCookiesNotification>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        public SetAuthCookiesNotificationHandler(IRepository<User> userRepo, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache)
        {
            _userRepo = userRepo;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task Handle(SetAuthCookiesNotification notification, CancellationToken cancellationToken)
        {
            var user =
                await _userRepo
                    .FirstOrDefaultAsync(
                        filter: b => b.UserName == notification.UserName,
                        include: source => source.Include(B => B.Role));

            if (user == null)
                return;


            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var token = JWTokenService.GenerateToken(user.UserName, user.Role.Title, ip, user.Email, user.PhoneNumber);


            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            String userCacheKey = $"user-{user.UserName}";

            _memoryCache.Set(userCacheKey, user, cacheEntryOptions);

            _contextAccessor.HttpContext.Response.Cookies.Append(SD.AuthToken, ProtectorData.Encrypt(token), new CookieOptions()
            {
                Expires = DateTimeOffset.MaxValue,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
