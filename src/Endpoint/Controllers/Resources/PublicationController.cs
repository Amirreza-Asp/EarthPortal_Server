using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Publications;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Shared;
using Domain.Entities.Resources;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase
    {
        private readonly IRepository<Publication> _repo;
        private readonly IMediator _mediator;

        public PublicationController(IRepository<Publication> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<SelectListItem>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);

        [AccessControl("Admin")]
        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreatePublicationCommand command, CancellationToken cancellationToken) =>
           await _mediator.Send(command, cancellationToken);

        [AccessControl("Admin")]
        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdatePublicationCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [AccessControl("Admin")]
        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemovePublicationCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
