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
            var footer = await _footerRepo.FirstOrDefaultAsync(b => true, cancellationToken: cancellationToken);

            var todaySeen = 1;
            var totalSeen = 1;
            List<OnlineUserData> onlineUsers;

            _memoryCache.TryGetValue<int>("todaySeen", out todaySeen);
            _memoryCache.TryGetValue<int>("totalSeen", out totalSeen);
            _memoryCache.TryGetValue<List<OnlineUserData>>("onlineUsers", out onlineUsers);


            var ip = HttpContext.Request.Headers["X-Forwarded-For"];

            return new FooterSummary
            {
                TodaySeen = todaySeen,
                TodayTotalSeen = 0,
                TotalSeen = totalSeen,
                UpdateAt = footer.LastUpdate,
                Ip = ip,
                OnlineUsers = onlineUsers == null ? 1 : onlineUsers.Count
            };
        }

    }
}
