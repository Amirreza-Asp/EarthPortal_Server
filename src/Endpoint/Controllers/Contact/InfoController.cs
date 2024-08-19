using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.CommonicationWays;
using Application.CQRS.Contact.Infos;
using Application.Models;
using Domain;
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
        public async Task<InfoSummary> Get(CancellationToken cancellationToken)
        {
            var res = await _repo.FirstOrDefaultAsync<InfoSummary>(b => true, cancellationToken: cancellationToken);
            res.GeoAddresses = res.GeoAddresses.OrderBy(b => b.Order).ToList();
            return res;
        }


        [HttpPut]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update(UpdateInfoCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPost]
        [Route("AddGeoAddress")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> AddGeoAddress([FromBody] AddGeoAddressCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);


        [HttpPut]
        [Route("UpdateGeoAddress")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> UpdateGeoAddress([FromBody] UpdateGeoAddressCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("RemoveGeoAddress")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> RemoveGeoAddress([FromQuery] RemoveGeoAddressCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
