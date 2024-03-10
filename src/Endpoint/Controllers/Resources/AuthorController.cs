using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Shared;
using Domain.Entities.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IRepository<Author> _repo;

        public AuthorController(IRepository<Author> repo)
        {
            _repo = repo;
        }



        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<SelectListItem>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);
    }
}
