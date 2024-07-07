using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account.User;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Account;
using Domain.Entities.Account;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [AccessControl("Admin")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _repo;
        private readonly IMediator _mediator;

        public UserController(IRepository<User> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ListActionResult<UserSummary>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<UserSummary>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("[action]")]
        public async Task<CommandResponse> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        public async Task<CommandResponse> Update([FromBody] UpdateUserCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("[action]")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveUserCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
