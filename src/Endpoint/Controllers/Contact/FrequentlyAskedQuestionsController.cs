﻿using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.FAQ;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Content;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
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
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Create([FromBody] CreateFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveFAQCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

    }
}
