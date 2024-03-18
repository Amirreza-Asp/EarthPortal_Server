using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Entities.Mutimedia;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Multimedia
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfographicController : ControllerBase
    {
        private readonly IRepository<Infographic> _repo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;

        public InfographicController(IRepository<Infographic> repo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator)
        {
            _repo = repo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<Infographic>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<Infographic>(query, cancellationToken: cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.InfographicPath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("Create")]
        [AccessControl("Admin")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromForm] CreateInfographicCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Remove")]
        [AccessControl("Admin")]
        [HttpDelete]
        public async Task<CommandResponse> Remove([FromQuery] RemoveInfographicCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
