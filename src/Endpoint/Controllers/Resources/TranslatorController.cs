using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Resources;
using Domain.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslatorController : ControllerBase
    {
        private readonly IRepository<Translator> _repo;

        public TranslatorController(IRepository<Translator> repo)
        {
            _repo = repo;
        }


        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<SelectListItem>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);
    }
}
