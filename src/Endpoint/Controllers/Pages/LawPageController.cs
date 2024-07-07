using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.LawPage;
using Application.Models;
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
        private readonly IMediator _mediator;

        public LawPageController(IRepository<LawPage> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<LawPage> Get(CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync<LawPage>(b => true, cancellationToken);

        [Route("[action]")]
        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateLawPageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
