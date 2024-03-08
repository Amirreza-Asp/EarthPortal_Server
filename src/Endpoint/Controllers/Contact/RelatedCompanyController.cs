using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Contact;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatedCompanyController : ControllerBase
    {
        private readonly IRepository<RelatedCompany> _repo;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IPhotoManager _photoManager;

        public RelatedCompanyController(IRepository<RelatedCompany> repo, IMediator mediator, IWebHostEnvironment env, IPhotoManager photoManager = null)
        {
            _repo = repo;
            _mediator = mediator;
            _hostEnv = env;
            _photoManager = photoManager;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<RelatedCompany>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<RelatedCompany>(query, cancellationToken: cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.RelatedCompanyPath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromForm] CreateRelatedCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromForm] UpdateRelatedCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveRelatedCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
