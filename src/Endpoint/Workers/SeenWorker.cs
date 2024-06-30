using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence;

namespace Endpoint.Workers
{
    public class SeenWorker : BackgroundService
    {
        private IServiceProvider _serviceProvider;
        private static List<(String connectionId, DateTime expired)> users = new List<(string connectionId, DateTime expired)>();

        public SeenWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            while (true)
            {
                users = users.Where(b => b.expired > DateTime.Now).ToList();
                var footer = await context.FooterPage.FirstOrDefaultAsync();

                if (footer == null)
                {
                    footer = new Domain.Entities.Pages.FooterPage();
                    context.FooterPage.Add(footer);
                    context.SaveChanges();
                }
                else
                {
                    memoryCache.TryGetValue("totalSeen", out int totalSeen);
                    memoryCache.TryGetValue("todaySeen", out int todaySeen);

                    if (footer.TotalSeen != totalSeen || footer.TodaySeen != todaySeen)
                    {
                        if (footer.TotalSeen > totalSeen)
                            memoryCache.Set("totalSeen", footer.TotalSeen, DateTimeOffset.MaxValue);
                        else
                            footer.TotalSeen = totalSeen;

                        footer.TodaySeen = todaySeen;

                        context.Update(footer);
                        context.SaveChanges();
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(10));

            }
        }
    }
}
