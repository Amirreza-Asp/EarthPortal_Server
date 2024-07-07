using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Notices;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Notices;
using Domain.Entities.Notices;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Notices
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsCategoryController : ControllerBase
    {
        private readonly IRepository<NewsCategory> _repo;
        private readonly IMediator _mediator;

        public NewsCategoryController(IRepository<NewsCategory> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<NewsCategorySummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<NewsCategorySummary>(query, cancellationToken: cancellationToken);


        [HttpGet]
        [Route("Find")]
        public async Task<NewsCategory> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync(b => b.Id == id, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("Create")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromBody] CreateNewsCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);



        [HttpPut]
        [Route("Update")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateNewsCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);



        [HttpDelete]
        [AccessControl("Admin")]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveNewsCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
