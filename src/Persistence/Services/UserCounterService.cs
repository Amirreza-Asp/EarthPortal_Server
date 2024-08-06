using Application.Contracts.Persistence.Services;
using Application.Models;
using Microsoft.AspNetCore.Http;
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

        public void Execute()
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
                    UpsertSeen(_context);
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

        void UpsertSeen(ApplicationDbContext context)
        {
            var statistics = context.Statistics.Where(b => b.Date.Date == DateTime.Now.Date).FirstOrDefault();

            if (statistics == null)
            {
                statistics = new Domain.Entities.Contact.Statistics(Guid.NewGuid(), 1, DateTime.Now.Date);
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
