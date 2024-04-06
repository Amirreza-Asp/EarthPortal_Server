using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Broadcasts;
using Application.CQRS.Resources.Brodcasts;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Resources;
using Domain.Entities.Resources;
using Endpoint.CustomeAttributes;
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
        private readonly IFileManager _fileManager;

        public BroadcastController(IRepository<Broadcast> broadcastRepo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator, IFileManager fileManager)
        {
            _broadcastRepo = broadcastRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _fileManager = fileManager;
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



        [Route("Find")]
        [HttpGet]
        public async Task<BroadcastDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var book = await _broadcastRepo.FirstOrDefaultAsync<BroadcastDetails>(b => b.Id == id, cancellationToken);

            var filePath = _hostEnv.WebRootPath + SD.BroadcastFilePath + book.File;
            book.Size = Math.Round(_fileManager.GetSize(filePath, FileSize.MB), 2);
            return book;
        }

        [Route("DownloadFile")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var book = await _broadcastRepo.FirstOrDefaultAsync(b => b.Id == id);
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BroadcastFilePath}{book.File}";

            var fileBytes = System.IO.File.ReadAllBytes(path);
            string extension = Path.GetExtension(book.File);
            return File(fileBytes, "application/force-download", book.Title + extension);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromForm] CreateBroadcastCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromForm] UpdateBroadcastCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [AccessControl("Admin")]
        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveBroadcastCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
