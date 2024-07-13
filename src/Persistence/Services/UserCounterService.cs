using Application.Contracts.Persistence.Services;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Persistence.Services
{
    public class UserCounterService : IUserCounterService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _connectionLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public UserCounterService(IHttpContextAccessor accessor, ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _accessor = accessor;
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var connectionId = _accessor.HttpContext.Connection.Id;
            //var semaphore = _connectionLocks.GetOrAdd(connectionId, new SemaphoreSlim(1, 1));

            //await semaphore.WaitAsync(cancellationToken);
            try
            {
                _memoryCache.TryGetValue<List<OnlineUserData>>("onlineUsers", out List<OnlineUserData>? onlineUsers);

                if (onlineUsers == null)
                    onlineUsers = new List<OnlineUserData>();

                var exist = onlineUsers.Any(x => x.IsValid && x.Id == connectionId);

                onlineUsers = onlineUsers.Where(b => b.IsValid && b.Id != connectionId).ToList();
                onlineUsers.Add(new OnlineUserData(connectionId));
                _memoryCache.Set<List<OnlineUserData>>("onlineUsers", onlineUsers, DateTimeOffset.MaxValue);

                if (!exist)
                {
                    await UpdateTotalSeen(_context, cancellationToken);
                    _memoryCache.TryGetValue<int>("todaySeen", out int todaySeen);
                    _memoryCache.Set<int>("todaySeen", todaySeen + 1, new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, TimeSpan.Zero));
                }
            }
            finally
            {
                //semaphore.Release();
                // Clean up the semaphore if it's no longer needed
                //if (_connectionLocks.TryRemove(connectionId, out var existingSemaphore))
                //{
                //    existingSemaphore.Dispose();
                //}
            }

        }


        async Task UpdateTotalSeen(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var footer =
                      await _context.FooterPage.FirstAsync();

            footer.TotalSeen += 1;
            _context.FooterPage.Update(footer);
            _context.SaveChanges();
        }
    }
}
