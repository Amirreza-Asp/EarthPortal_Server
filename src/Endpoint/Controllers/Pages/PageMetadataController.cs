using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.PageMetadata;
using Application.Models;
using Application.Queries;
using Domain.Dtos.Pages;
using Domain.Dtos.Shared;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PageMetadataController : ControllerBase
    {
        private readonly IRepository<PageMetadata> _pageMetadataRepo;
        private readonly IMediator _mediator;

        public PageMetadataController(IRepository<PageMetadata> pageMetadataRepo, IMediator mediator)
        {
            _pageMetadataRepo = pageMetadataRepo;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<PageMetadataSummary>> GetAll(CancellationToken cancellationToken) =>
            await _pageMetadataRepo.GetAllAsync<PageMetadataSummary>(b => true, cancellationToken);

        [HttpGet]
        public async Task<List<SelectListItem>> SearchForKeywords([FromQuery] KeywordFilteringQuery query, CancellationToken cancelationToken) =>
            await _mediator.Send(query);

        [HttpPost]
        public async Task<ListActionResult<PageMetadataSummary>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _pageMetadataRepo.GetAllAsync<PageMetadataSummary>(query, cancellationToken: cancellationToken);

        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreatePageMetadataCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        public async Task<CommandResponse> Update([FromBody] UpdatePageMetadataCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        public async Task<CommandResponse> Delete([FromQuery] DeletePageMetadataCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
