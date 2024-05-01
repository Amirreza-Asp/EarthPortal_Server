using Application.Contracts.Persistence.Repositories;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class FooterController : ControllerBase
    {
        private readonly IRepository<FooterPage> _footer;

        public FooterController(IRepository<FooterPage> footer)
        {
            _footer = footer;
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<FooterSummary> Get(CancellationToken cancellationToken) =>
            new FooterSummary
            {
                TodaySeen = 2133,
                TodayTotalSeen = 2133,
                TotalSeen = 2133,
                OnlineUsers = 57,
                UpdateAt = DateTime.UtcNow,
                Ip = HttpContext.Connection.RemoteIpAddress.ToString()
            };
    }
}
