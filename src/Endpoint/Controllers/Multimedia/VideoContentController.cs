using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Multimedia;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Multimedia
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoContentController : ControllerBase
    {
        private readonly IRepository<VideoContent> _repo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;

        public VideoContentController(IRepository<VideoContent> repo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator)
        {
            _repo = repo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<VideoContentSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<VideoContentSummary>(query, cancellationToken: cancellationToken);


        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.VideoContentPath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }



        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromBody] CreateVideoContentCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromBody] UpdateVideoContentCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveVideoContentCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
