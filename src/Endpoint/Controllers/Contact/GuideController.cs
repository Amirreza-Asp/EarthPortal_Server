using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Contact;
using Domain.Entities.Contact;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuideController : ControllerBase
    {
        private readonly IRepository<Guide> _repo;

        public GuideController(IRepository<Guide> repo)
        {
            _repo = repo;
        }


        [HttpGet]
        [Route("Summary")]
        public async Task<ListActionResult<GuideSummary>> Summary(CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<GuideSummary>(new GridQuery { Size = int.MaxValue }, cancellationToken: cancellationToken);
        }


        [HttpGet]
        [Route("Find")]
        public async Task<Guide> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync(b => b.Id == id, cancellationToken: cancellationToken);
        }
    }
}
