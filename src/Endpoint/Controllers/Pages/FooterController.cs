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

        public FooterController(IRepository<FooterPage> footerRepo, IMemoryCache memoryCache, IUserCounterService userCounterService)
        {
            _footerRepo = footerRepo;
            _memoryCache = memoryCache;
            _userCounterService = userCounterService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<FooterSummary> Get(CancellationToken cancellationToken)
        {
            await _userCounterService.ExecuteAsync(default);
            var footer = await _footerRepo.FirstOrDefaultAsync(b => true, cancellationToken: cancellationToken);

            List<OnlineUserData> onlineUsers;

            _memoryCache.TryGetValue<int>("todaySeen", out int todaySeen);
            _memoryCache.TryGetValue<List<OnlineUserData>>("onlineUsers", out onlineUsers);


            var ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(",")[0];

            return new FooterSummary
            {
                TodaySeen = Math.Max(todaySeen, 1),
                TodayTotalSeen = 0,
                TotalSeen = footer.TotalSeen,
                UpdateAt = footer.LastUpdate,
                Ip = ip,
                OnlineUsers = onlineUsers == null ? 1 : onlineUsers.Count
            };
        }

    }
}
