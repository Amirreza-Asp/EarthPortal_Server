using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Domain.Entities.Pages;
using Endpoint.CustomeAttributes;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageRepository _repo;

        public HomePageController(IHomePageRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<HomePage> Get(CancellationToken cancellationToken) =>
            await _repo.GetAsync(cancellationToken);

        [HttpPost]
        [AccessControl("Admin")]
        [Route("[action]")]
        public async Task<CommandResponse> ChangeHeader([FromBody] HomeHeader header, CancellationToken cancellationToken) =>
            await _repo.ChangeHeaderAsync(header, cancellationToken);

        [HttpPost]
        [AccessControl("Admin")]
        [Route("[action]")]
        public async Task<CommandResponse> ChangeWork([FromBody] HomeWork work, CancellationToken cancellationToken) =>
            await _repo.ChangeWorkAsync(work, cancellationToken);
    }
}
