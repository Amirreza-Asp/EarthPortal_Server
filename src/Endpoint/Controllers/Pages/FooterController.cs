using Application.Contracts.Persistence.Repositories;
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

        public FooterController(IRepository<FooterPage> footerRepo, IMemoryCache memoryCache)
        {
            _footerRepo = footerRepo;
            _memoryCache = memoryCache;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<FooterSummary> Get(CancellationToken cancellationToken)
        {
            var footer = await _footerRepo.FirstOrDefaultAsync(b => true, cancellationToken: cancellationToken);

            var onlineUsers = 0;
            _memoryCache.TryGetValue<int>("onlineUsers", out onlineUsers);

            return new FooterSummary
            {
                TodaySeen = Math.Max(footer.TodaySeen, 1),
                TodayTotalSeen = 0,
                TotalSeen = Math.Max(footer.TotalSeen, 1),
                UpdateAt = footer.LastUpdate,
                Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                OnlineUsers = Math.Max(onlineUsers, 1)
            };
        }

    }
}
