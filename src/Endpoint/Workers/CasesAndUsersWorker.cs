
using Application.Contracts.Persistence.Repositories;
using Application.Contracts.Persistence.Services;

namespace Endpoint.BackgroundServices
{
    public class CasesAndUsersWorker : BackgroundService
    {

        private IServiceProvider _serviceProvider;
        private IIranelandService _iranelandService;
        private IHomePageRepository _homePageRepo;
        private ILogger<CasesAndUsersWorker> _logger;

        public CasesAndUsersWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            _homePageRepo = scope.ServiceProvider.GetService<IHomePageRepository>();
            _logger = scope.ServiceProvider.GetService<ILogger<CasesAndUsersWorker>>();
            _iranelandService = scope.ServiceProvider.GetService<IIranelandService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("CasesAndUsers running at: {time}", DateTimeOffset.Now);
                var data = await _iranelandService.GetCasesAndUsersAsync(stoppingToken);

                var result = await _homePageRepo.UpdateCasesAndUsersAsync(data);
                if (result.Status == 200)
                    await Task.Delay(1000 * 60 * 60, stoppingToken);
                else
                    await Task.Delay(1000 * 60 * 5, stoppingToken);
            }
        }
    }
}
