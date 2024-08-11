using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.LawPage;
using Application.Models;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using Domain.Entities.Regulation;
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
        private readonly IRepository<Law> _lawRepo;
        private readonly IMediator _mediator;

        public LawPageController(IRepository<LawPage> repo, IMediator mediator, IRepository<Law> laws)
        {
            _repo = repo;
            _mediator = mediator;
            _lawRepo = laws;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<LawPageDto> Get(CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<LawPageDto>(b => true, cancellationToken);
            data.LawCount = await _lawRepo.CountAsync(b => true, cancellationToken);
            return data;
        }

        [Route("[action]")]
        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateLawPageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
