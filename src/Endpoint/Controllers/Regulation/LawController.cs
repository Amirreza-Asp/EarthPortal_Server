using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Regulation;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Regulation
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawController : ControllerBase
    {
        private readonly ILawRepository _lawRepository;
        private readonly IMediator _mediator;

        public LawController(ILawRepository lawRepository, IMediator mediator)
        {
            _lawRepository = lawRepository;
            _mediator = mediator;
        }


        [Route("PagenationSpecificQuery")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSpecificQuery([FromBody] LawPagenationQuery query, CancellationToken cancellationToken) =>
            await _lawRepository.PagenationSummaryAsync(query, cancellationToken);

        [Route("PagenationSummary")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _lawRepository.GetAllAsync<LawSummary>(query, cancellationToken: cancellationToken);

        [Route("Find")]
        [HttpGet]
        public async Task<LawDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _lawRepository.FirstOrDefaultAsync<LawDetails>(b => b.Id == id, cancellationToken);

        [Route("Create")]
        [HttpPost]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromBody] CreateLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Update")]
        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Remove")]
        [HttpDelete]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
