using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Persistence;

namespace Endpoint.Hubs
{
    public class OnlineHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public OnlineHub(IMemoryCache memoryCache, ApplicationDbContext context)
        {
            _memoryCache = memoryCache;
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            UpdateOnlineUsers(1);
            UpdateTodaySeen();
            UpdateTotalSeen();

            return base.OnConnectedAsync();
        }



        public override Task OnDisconnectedAsync(Exception? exception)
        {
            UpdateOnlineUsers(-1);

            return base.OnDisconnectedAsync(exception);
        }


        private void UpdateOnlineUsers(int number)
        {
            int onlineUsers = 0;

            if (!_memoryCache.TryGetValue<int>("onlineUsers", out onlineUsers))
                _memoryCache.Set<int>("onlineUsers", 1, DateTimeOffset.MaxValue);
            else
                _memoryCache.Set<int>("onlineUsers", onlineUsers + number, DateTimeOffset.MaxValue);
        }

        private void UpdateTodaySeen()
        {
            var footer = _context.FooterPage.First();

            if (footer.Today.Date != DateTime.Now.Date)
            {
                footer.TodaySeen = 1;
                footer.Today = DateTime.Now;
            }
            else
                footer.TodaySeen += 1;

            _context.FooterPage.Update(footer);
            _context.SaveChanges();
        }

        private void UpdateTotalSeen()
        {
            var footer = _context.FooterPage.First();
            footer.TotalSeen += 1;
            _context.FooterPage.Update(footer);
            _context.SaveChanges();
        }
    }
}
