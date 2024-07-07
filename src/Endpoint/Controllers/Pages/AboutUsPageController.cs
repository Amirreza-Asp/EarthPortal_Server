using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.AboutUsPage;
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
    public class AboutUsPageController : ControllerBase
    {
        private readonly IRepository<AboutUsPage> _repo;
        private readonly IMediator _mediator;

        public AboutUsPageController(IRepository<AboutUsPage> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<AboutUsPage> Get(CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync<AboutUsPage>(b => true, cancellationToken: cancellationToken);

        [Route("[action]")]
        [HttpPost]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromBody] UpdateAboutUsPageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
