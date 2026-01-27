using Application.Contracts.Persistence.Repositories;
using Application.Contracts.Persistence.Services;
using Application.Models;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class FooterController : ControllerBase
    {
        private readonly IRepository<FooterPage> _footerRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserCounterService _userCounterService;
        private readonly IStatisticsRepository _statisticsRepository;

        public FooterController(
            IRepository<FooterPage> footerRepo,
            IMemoryCache memoryCache,
            IUserCounterService userCounterService,
            IStatisticsRepository statisticsRepository
        )
        {
            _footerRepo = footerRepo;
            _memoryCache = memoryCache;
            _userCounterService = userCounterService;
            _statisticsRepository = statisticsRepository;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<FooterSummary> Get(CancellationToken cancellationToken)
        {
            _userCounterService.Execute();
            var footer = await _footerRepo.FirstOrDefaultAsync(
                b => true,
                cancellationToken: cancellationToken
            );

            _memoryCache.TryGetValue("onlineUsers", out List<OnlineUserData>? onlineUsers);

            var ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(",")[0];

            var todaySeen = await _statisticsRepository.GetTodaySeen();
            var totalSeen = await _statisticsRepository.GetTotalSeen();
            var yearSeen = await _statisticsRepository.GetYearSeen();
            var MonthSeen = await _statisticsRepository.GetMonthSeen();

            return new FooterSummary
            {
                TodaySeen = Math.Max(todaySeen, 1),
                TotalSeen = Math.Max(totalSeen, 1),
                YearSeen = Math.Max(yearSeen, 1),
                MonthSeen = Math.Max(MonthSeen, 1),
                UpdateAt = footer.LastUpdate,
                Ip = ip,
                OnlineUsers = onlineUsers == null ? 1 : onlineUsers.Count
            };
        }
    }
}
