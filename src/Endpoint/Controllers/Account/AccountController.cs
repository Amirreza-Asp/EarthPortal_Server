using Application.CQRS.Account;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<CommandResponse> Login([FromBody] LoginCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
