using Application.Contracts.Persistence.Repositories;
using Domain.Dtos.Contact;
using Domain.Entities.Contact;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IRepository<Info> _repo;

        public InfoController(IRepository<Info> repo)
        {
            _repo = repo;
        }



        [HttpGet]
        public async Task<InfoSummary> Get(CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync<InfoSummary>(b => true, cancellationToken: cancellationToken);
    }
}
