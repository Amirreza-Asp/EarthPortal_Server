using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.LawPage;
using Application.Models;
using Domain;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawPageController : ControllerBase
    {
        private readonly IRepository<LawPage> _repo;
        private readonly ILawRepository _lawRepo;
        private readonly IMediator _mediator;

        public LawPageController(IRepository<LawPage> repo, IMediator mediator, ILawRepository lawRepo)
        {
            _repo = repo;
            _mediator = mediator;
            _lawRepo = lawRepo;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<LawPageDto> Get(CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<LawPageDto>(b => true, cancellationToken);
            data.LawCount = await _lawRepo.CountAsync(cancellationToken);
            data.LastModifiedAt = await _lawRepo.GetLastModifiedAsync(cancellationToken);
            return data;
        }

        [Route("[action]")]
        [HttpPut]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateLawPageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
