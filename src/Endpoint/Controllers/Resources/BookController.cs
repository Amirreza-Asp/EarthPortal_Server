using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Books;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Resources;
using Domain.Entities.Resources;
using Endpoint.CustomeAttributes;
using Endpoint.Filters;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    [DisableController] // disabling controller for now
    public class BookController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;
        private readonly IFileManager _fileManager;

        public BookController(
            IRepository<Book> bookRepo,
            IPhotoManager photoManager,
            IWebHostEnvironment hostEnv,
            IMediator mediator,
            IFileManager fileManager
        )
        {
            _bookRepo = bookRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _fileManager = fileManager;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<BookSummary>> PagenationSummary(
            [FromBody] GridQuery query,
            CancellationToken cancellationToken
        )
        {
            return await _bookRepo.GetAllAsync<BookSummary>(
                query,
                cancellationToken: cancellationToken
            );
        }

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage(
            [FromQuery] ImageQuery query,
            CancellationToken cancellationToken
        )
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BookImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(
                path,
                query.Width,
                query.Height,
                cancellationToken
            );

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("Find")]
        [HttpGet]
        public async Task<BookDetails> Find(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        )
        {
            var book = await _bookRepo.FirstOrDefaultAsync<BookDetails>(
                b => b.Id == id,
                cancellationToken
            );

            var filePath = _hostEnv.WebRootPath + SD.BookFilePath + book.File;
            book.Size = Math.Round(_fileManager.GetSize(filePath, FileSize.MB), 2);
            return book;
        }

        [Route("DownloadFile")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        )
        {
            var book = await _bookRepo.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var baseDir = Path.GetFullPath(Path.Combine(_hostEnv.WebRootPath, SD.BookFilePath));

            var safeFileName = Path.GetFileName(book.File);

            var fullPath = Path.GetFullPath(Path.Combine(baseDir, safeFileName));

            if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var downloadName = book.Title + Path.GetExtension(safeFileName);

            return PhysicalFile(fullPath, "application/octet-stream", downloadName);
        }

        [Route("[action]")]
        [HttpGet]
        public String DownloadFileBase64(
            [FromQuery] String file,
            CancellationToken cancellationToken
        )
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.BookFilePath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(fileBytes);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Create(
            [FromForm] CreateBookCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update(
            [FromForm] UpdateBookCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Remove(
            [FromQuery] RemoveBookCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
