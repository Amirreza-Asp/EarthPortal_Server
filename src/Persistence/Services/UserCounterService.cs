using Application.Contracts.Persistence.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.Services
{
    public class UserCounterService : IUserCounterService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public UserCounterService(IHttpContextAccessor accessor, ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _accessor = accessor;
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
                ip = _accessor.HttpContext.Request.Headers["X-Forwarded-For"];


            bool updated = false;

            var ipsList = _memoryCache.Get<List<(String ip, DateTime connectedAt)>>("ips");
            ipsList = ipsList?.Where(b => b.ip != "::1").ToList();

            if (ipsList == null || !ipsList.Any())
            {
                ipsList = new List<(string ip, DateTime connectedAt)>
                {
                    (ip , DateTime.Now)
                };
                _memoryCache.Set<List<(String ip, DateTime connectedAt)>>("ips", ipsList);

                int todaySeen = 0;
                int totalSeen = 0;

                _memoryCache.TryGetValue("todaySeen", out todaySeen);
                _memoryCache.TryGetValue("totalSeen", out totalSeen);

                _memoryCache.Set("todaySeen", todaySeen + 1, new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 0, TimeSpan.Zero));
                _memoryCache.Set("totalSeen", totalSeen + 1, DateTimeOffset.MaxValue);
            }
            else
            {
                ipsList = ipsList.Where(b => b.connectedAt.AddMinutes(15) > DateTime.Now).ToList();

                if (!ipsList.Any(b => b.ip == ip))
                {
                    updated = true;
                    ipsList.Add((ip, DateTime.Now));

                    int todaySeen = 0;
                    int totalSeen = 0;

                    _memoryCache.TryGetValue("todaySeen", out todaySeen);
                    _memoryCache.TryGetValue("totalSeen", out totalSeen);

                    _memoryCache.Set("todaySeen", todaySeen + 1, new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 0, TimeSpan.Zero));
                    _memoryCache.Set("totalSeen", totalSeen + 1, DateTimeOffset.MaxValue);
                }

                _memoryCache.Set<List<(String ip, DateTime connectedAt)>>("ips", ipsList, DateTimeOffset.MaxValue);
            }


            _memoryCache.Set<int>("onlineUsers", ipsList.Count, DateTimeOffset.MaxValue);

        }
    }
}
