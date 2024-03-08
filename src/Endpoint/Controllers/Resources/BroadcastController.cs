using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Resources;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastController : ControllerBase
    {
        private readonly IRepository<Broadcast> _broadcastRepo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;

        public BroadcastController(IRepository<Broadcast> broadcastRepo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator)
        {
            _broadcastRepo = broadcastRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<BroadcastSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _broadcastRepo.GetAllAsync<BroadcastSummary>(query, cancellationToken: cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BroadcastImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

    }
}
