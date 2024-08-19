using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Regulation;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Regulation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalTypeController : ControllerBase
    {
        private readonly IRepository<ApprovalType> _repo;
        private readonly IMediator _mediator;

        public ApprovalTypeController(IRepository<ApprovalType> repo, IMediator mediator)
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
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Create([FromBody] CreateApprovalTypeCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateApprovalTypeCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveApprovalTypeCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
