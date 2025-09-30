using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Notices;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Notices;
using Domain.Entities.Notices;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Notices
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeCategoryController : ControllerBase
    {
        private readonly IRepository<NoticeCategory> _repo;
        private readonly IMediator _mediator;

        public NoticeCategoryController(IRepository<NoticeCategory> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<NoticeCategorySummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<NoticeCategorySummary>(query, cancellationToken: cancellationToken);


        [HttpGet]
        [Route("Find")]
        public async Task<NoticeCategory> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync(b => b.Id == id, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("Create")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Create([FromBody] CreateNoticeCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);



        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateNoticeCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);



        [HttpDelete]
        [AccessControl(SD.AdminRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveNoticeCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
