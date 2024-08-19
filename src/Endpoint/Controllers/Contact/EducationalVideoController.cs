using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalVideoController : ControllerBase
    {
        private readonly IRepository<EducationalVideo> _repo;
        private readonly IMediator _mediator;



        public EducationalVideoController(IRepository<EducationalVideo> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<EducationalVideo>> PagenationSummary(GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<EducationalVideo>(query, cancellationToken: cancellationToken);


        [HttpPost]
        [AccessControl(SD.AdminRole)]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateEducationalVideoCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [AccessControl(SD.AdminRole)]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateEducationalVideoCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [AccessControl(SD.AdminRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveEducationalVideoCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
