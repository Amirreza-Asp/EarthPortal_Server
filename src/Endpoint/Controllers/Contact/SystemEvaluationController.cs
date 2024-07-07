using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.SystemEvaluations;
using Application.Models;
using Domain.Dtos.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemEvaluationController : ControllerBase
    {
        private readonly ISystemEvaluationRepository _repo;
        private readonly IMediator _mediator;

        public SystemEvaluationController(ISystemEvaluationRepository repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [HttpGet]
        [AccessControl("Admin")]
        public async Task<SystemEvaluationDetails> Get(CancellationToken cancellationToken) =>
            await _repo.GetAsync(cancellationToken);



        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> SystemEvaluation([FromBody] CreateSystemEvaluationCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
