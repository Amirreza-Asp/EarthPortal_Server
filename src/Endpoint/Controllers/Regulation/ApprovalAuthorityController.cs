using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.ApprovalAuthorities;
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
    public class ApprovalAuthorityController : ControllerBase
    {
        private readonly IRepository<ApprovalAuthority> _repo;
        private readonly IMediator _mediator;

        public ApprovalAuthorityController(IRepository<ApprovalAuthority> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("SelectListPagenation")]
        public async Task<ListActionResult<SelectListItem>> SelectListPagenation([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateApprovalAuthorityCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateApprovalAuthorityCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveApprovalAuthorityCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
