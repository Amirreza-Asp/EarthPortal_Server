using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Translators;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Resources;
using Domain.Dtos.Shared;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslatorController : ControllerBase
    {
        private readonly IRepository<Translator> _repo;
        private readonly IMediator _mediator;

        public TranslatorController(IRepository<Translator> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<SelectListItem>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("Create")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Create([FromBody] CreateTranslatorCommand command, CancellationToken cancellationToken) =>
         await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateTranslatorCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveTranslatorCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
