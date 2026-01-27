using System.Collections.Concurrent;
using Application.Contracts.Persistence.Services;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.Services
{
    public class UserCounterService : IUserCounterService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _connectionLocks =
            new ConcurrentDictionary<string, SemaphoreSlim>();

        public UserCounterService(
            IHttpContextAccessor accessor,
            ApplicationDbContext context,
            IMemoryCache memoryCache
        )
        {
            _accessor = accessor;
            _context = context;
            _memoryCache = memoryCache;
        }

        public void Execute()
        {
            var connectionId =
                _accessor.HttpContext.Request.Cookies["CustomConnectionId"]
                ?? Guid.NewGuid().ToString();

            if (!_memoryCache.TryGetValue("onlineUsers", out List<OnlineUserData>? onlineUsers))
            {
                onlineUsers = new List<OnlineUserData>();
            }

            bool exist = onlineUsers.Any(x => x.IsValid && x.Id == connectionId);

            onlineUsers.RemoveAll(b => !b.IsValid || b.Id == connectionId);

            onlineUsers.Add(new OnlineUserData(connectionId));

            _memoryCache.Set("onlineUsers", onlineUsers, DateTimeOffset.MaxValue);

            if (!exist)
            {
                UpsertSeen(_context);
                _accessor.HttpContext.Response.Cookies.Append(
                    "CustomConnectionId",
                    connectionId,
                    new CookieOptions()
                    {
                        Expires = DateTime.Now.AddHours(2),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict
                    }
                );
            }
        }

        void UpsertSeen(ApplicationDbContext context)
        {
            var statistics = context
                .Statistics.Where(b => b.Date.Date == DateTime.Now.Date)
                .FirstOrDefault();

            if (statistics == null)
            {
                statistics = new Domain.Entities.Contact.Statistics(
                    Guid.NewGuid(),
                    1,
                    DateTime.Now.Date
                );
                context.Statistics.Add(statistics);
            }
            else
            {
                statistics.Seen = statistics.Seen + 1;
                context.Statistics.Update(statistics);
            }

            context.SaveChanges();
        }
    }
}
