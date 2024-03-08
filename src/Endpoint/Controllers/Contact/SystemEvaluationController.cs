using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.SystemEvaluations;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemEvaluationController : ControllerBase
    {
        private readonly IRepository<SystemEvaluation> _repo;
        private readonly IMediator _mediator;

        public SystemEvaluationController(IRepository<SystemEvaluation> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> SystemEvaluation([FromBody] CreateSystemEvaluationCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
