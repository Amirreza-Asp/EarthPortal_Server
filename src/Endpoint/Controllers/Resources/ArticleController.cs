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
    public class ArticleController : ControllerBase
    {
        private readonly IRepository<Article> _articleRepo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;

        public ArticleController(IRepository<Article> bookRepo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator)
        {
            _articleRepo = bookRepo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
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
    }
}
