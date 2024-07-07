using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.CommonicationWays;
using Application.CQRS.Contact.Infos;
using Application.Models;
using Domain.Dtos.Contact;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IRepository<Info> _repo;
        private readonly IMediator _mediator;

        public InfoController(IRepository<Info> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }



        [HttpGet]
        public async Task<InfoSummary> Get(CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync<InfoSummary>(b => true, cancellationToken: cancellationToken);


        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update(UpdateInfoCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPost]
        [Route("AddGeoAddress")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromBody] AddGeoAddressCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);


        [HttpDelete]
        [Route("RemoveGeoAddress")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveGeoAddressCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
