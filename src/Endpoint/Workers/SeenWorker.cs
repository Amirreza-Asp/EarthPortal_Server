using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence;

namespace Endpoint.Workers
{
    public class SeenWorker : BackgroundService
    {
        private IServiceProvider _serviceProvider;

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
                await Task.Delay(1000 * 60 * 5);
                var footer = await context.FooterPage.FirstOrDefaultAsync();

                if (footer == null)
                {
                    footer = new Domain.Entities.Pages.FooterPage();
                    context.FooterPage.Add(footer);
                    context.SaveChanges();
                }
                else
                {
                    var totalSeen = 0;
                    var todaySeen = 0;

                    memoryCache.TryGetValue("totalSeen", out totalSeen);
                    memoryCache.TryGetValue("todaySeen", out todaySeen);

                    if (footer.TodaySeen != todaySeen || footer.TotalSeen != totalSeen)
                    {
                        footer.TodaySeen = todaySeen;
                        footer.TotalSeen = totalSeen;


                        context.Update(footer);
                        context.SaveChanges();
                    }
                }

            }
        }
    }
}
