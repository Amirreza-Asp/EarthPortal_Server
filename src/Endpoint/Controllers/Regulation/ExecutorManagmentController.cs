using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.ExecutorManagements;
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
    public class ExecutorManagmentController : ControllerBase
    {

        private readonly IRepository<ExecutorManagment> _repo;
        private readonly IMediator _mediator;

        public ExecutorManagmentController(IRepository<ExecutorManagment> repo, IMediator mediator)
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
        public async Task<CommandResponse> Create([FromBody] CreateExecutorManagementCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateExecutorManagementCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveExecutorManagementCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
