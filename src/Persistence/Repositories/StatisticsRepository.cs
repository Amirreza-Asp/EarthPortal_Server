using Application.Contracts.Persistence.Repositories;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public StatisticsRepository(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<int> GetMonthSeen()
        {
            var firstDay = DateTimeExtension.GetFirstDayOfMonth();
            var lastDay = DateTimeExtension.GetLastDayOfMonth();

            return
                await _context.Statistics
                    .Where(b => b.Date.Date >= firstDay.Date && b.Date.Date <= lastDay.Date)
                    .SumAsync(b => b.Seen);
        }

        public async Task<int> GetTodaySeen()
        {
            return
                await _context.Statistics
                    .Where(b => b.Date.Date == DateTime.Now.Date)
                    .Select(b => b.Seen)
                    .FirstOrDefaultAsync();
        }

        public async Task<long> GetTotalSeen()
        {
            if (_memoryCache.TryGetValue("EarthTotalSeen", out long total))
                return total;

            total =
                 await _context.Statistics
                     .SumAsync(b => b.Seen);

            _memoryCache.Set("EarthTotalSeen", total, DateTimeOffset.Now.AddHours(1));
            return total;
        }

        public async Task<long> GetYearSeen()
        {
            var firstDay = DateTimeExtension.GetFirstDayOfYear();
            var lastDay = DateTimeExtension.GetLastDayOfYear();

            return
                 await _context.Statistics
                     .Where(b => b.Date.Date >= firstDay.Date && b.Date.Date <= lastDay.Date)
                     .SumAsync(b => b.Seen);
        }
    }
}
