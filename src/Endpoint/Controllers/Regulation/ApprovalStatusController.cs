using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Shared;
using Domain.Entities.Regulation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Regulation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalStatusController : ControllerBase
    {
        private readonly IRepository<ApprovalStatus> _repo;
        private readonly IMediator _mediator;

        public ApprovalStatusController(IRepository<ApprovalStatus> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("SelectListPagenation")]
        public async Task<ListActionResult<SelectListItem>> SelectListPagenation([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);


        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateApprovalStatusCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateApprovalStatusCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveApprovalStatusCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
