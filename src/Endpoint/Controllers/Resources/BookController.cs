using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Books;
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
    public class BookController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;
        private readonly IFileManager _fileManager;

        public BookController(IRepository<Book> bookRepo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator, IFileManager fileManager)
        {
            _bookRepo = bookRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _fileManager = fileManager;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<BookSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _bookRepo.GetAllAsync<BookSummary>(query, cancellationToken: cancellationToken);
        }

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BookImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("Find")]
        [HttpGet]
        public async Task<BookDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.FirstOrDefaultAsync<BookDetails>(b => b.Id == id, cancellationToken);

            var filePath = _hostEnv.WebRootPath + SD.BookFilePath + book.File;
            book.Size = Math.Round(_fileManager.GetSize(filePath, FileSize.MB), 2);
            return book;
        }

        [Route("DownloadFile")]
        [HttpGet]
        public FileResult DownloadFile([FromQuery] String file, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BookFilePath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, "application/force-download", file);
        }

        [Route("[action]")]
        [HttpGet]
        public String DownloadFileBase64([FromQuery] String file, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BookFilePath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(fileBytes);
        }


        [AccessControl("Admin")]
        [HttpPost]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromForm] CreateBookCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [AccessControl("Admin")]
        [HttpPut]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromForm] UpdateBookCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [AccessControl("Admin")]
        [HttpDelete]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveBookCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

    }
}
