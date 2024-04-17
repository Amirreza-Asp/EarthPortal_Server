using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Resources.Articles;
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
    public class ArticleController : ControllerBase
    {
        private readonly IRepository<Article> _articleRepo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;
        private readonly IFileManager _fileManager;

        public ArticleController(IRepository<Article> bookRepo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator, IFileManager fileManager)
        {
            _articleRepo = bookRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _fileManager = fileManager;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<ArticleSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _articleRepo.GetAllAsync<ArticleSummary>(query, cancellationToken: cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.ArticleImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("Find")]
        [HttpGet]
        public async Task<ArticleDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var book = await _articleRepo.FirstOrDefaultAsync<ArticleDetails>(b => b.Id == id, cancellationToken);

            var filePath = _hostEnv.WebRootPath + SD.ArticleFilePath + book.File;
            book.Size = Math.Round(_fileManager.GetSize(filePath, FileSize.MB), 2);
            return book;
        }

        [Route("DownloadFile")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var book = await _articleRepo.FirstOrDefaultAsync(b => b.Id == id);
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.ArticleFilePath}{book.File}";

            var fileBytes = System.IO.File.ReadAllBytes(path);
            string extension = Path.GetExtension(book.File);
            return File(fileBytes, "application/force-download", book.Title + extension);
        }

        [Route("[action]")]
        [HttpGet]
        public String DownloadFileBase64([FromQuery] String file, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.ArticleFilePath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(fileBytes);
        }



        [HttpPost]
        [Route("Create")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromForm] CreateArticleCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [HttpPut]
        [AccessControl("Admin")]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromForm] UpdateArticleCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [HttpDelete]
        [Route("Remove")]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveArticleCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
