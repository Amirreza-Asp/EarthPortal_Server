using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.Guids;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Contact;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuideController : ControllerBase
    {
        private readonly IRepository<Guide> _repo;
        private readonly IMediator _mediator;

        public GuideController(IRepository<Guide> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpGet]
        [Route("Summary")]
        public async Task<ListActionResult<GuideSummary>> Summary(CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<GuideSummary>(new GridQuery { Size = int.MaxValue }, cancellationToken: cancellationToken);
        }


        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<GuideSummary>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<GuideSummary>(query, cancellationToken: cancellationToken);


        [HttpGet]
        [Route("Find")]
        public async Task<Guide> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromBody] CreateGuideCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateGuideCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveGuideCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
