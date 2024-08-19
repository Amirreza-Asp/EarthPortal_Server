using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.Goals;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IRepository<Goal> _repo;
        private readonly IMediator _mediator;

        public GoalController(IRepository<Goal> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<Goal>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<Goal>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [AccessControl(SD.AdminRole)]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateGoalCommand command, CancellationToken cancellationToken) =>
         await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [AccessControl(SD.AdminRole)]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateGoalCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [AccessControl(SD.AdminRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveGoalCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
