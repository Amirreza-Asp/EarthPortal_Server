using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.FAQ;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Content;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Content
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrequentlyAskedQuestionsController : ControllerBase
    {
        private readonly IRepository<FrequentlyAskedQuestions> _repo;
        private readonly IMediator _mediator;

        public FrequentlyAskedQuestionsController(IRepository<FrequentlyAskedQuestions> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<FrequentlyAskedQuestionsSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
                await _repo.GetAllAsync<FrequentlyAskedQuestionsSummary>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

    }
}
