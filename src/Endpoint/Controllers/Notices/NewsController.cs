using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Notices;
using Application.CQRS.Notices.Notifications;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Notices
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _newsRepository;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;
        private readonly IRepository<Link> _linkRepo;

        public NewsController(INewsRepository newsRepository, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator, IRepository<Link> linkRepo)
        {
            _newsRepository = newsRepository;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _linkRepo = linkRepo;
        }


        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<NewsSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            query.Sorted = new SortModel[] { new SortModel { column = "dateOfRegisration", desc = true }, new SortModel { column = "order", desc = false } };
            return await _newsRepository.GetAllAsync<NewsSummary>(query, cancellationToken: cancellationToken);
        }


        [HttpPost]
        [Route("FilteringPagenation")]
        public async Task<ListActionResult<NewsSummary>> FilteringPagenation([FromBody] NewsFilteringPagenationQuery query, CancellationToken cancellationToken)
        {
            return
                await _newsRepository.GetAllAsync<NewsSummary>(
                    new GridQuery { Page = query.Page, Size = query.Size, Sorted = new SortModel[] { new SortModel { column = "dateOfRegisration", desc = true } } },
                    b => (!query.LinksId.Any() || b.Links.Where(b => query.LinksId.Contains(b.LinkId)).Any()) &&
                         (String.IsNullOrEmpty(query.Title) || b.Title.Contains(query.Title) || b.Headline.Contains(query.Title) || b.Links.Any(s => s.Link.Value.Contains(query.Title))),
                    cancellationToken);
        }


        [HttpGet]
        [Route("SearchByKeyword")]
        public async Task<ListActionResult<NewsSummary>> SearchByKeyword([FromQuery] String keyword, [FromQuery] int page, [FromQuery] int size, CancellationToken cancellationToken)
        {
            return
                await _newsRepository.SearchByKeywordAsync(keyword, page, size, cancellationToken);
        }

        [HttpGet]
        [Route("Find")]
        public async Task<NewsDetails?> Find([FromQuery] int shortLink, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new IncreaseNewsSeenNotification(shortLink));
            var news =
                await _newsRepository.FirstOrDefaultAsync<NewsDetails>(
                    filters: b => b.ShortLink == shortLink,
                    cancellationToken: cancellationToken);

            if (news != null)
            {
                news.RelatedNews = await _newsRepository.RelatedNewsAsync(shortLink, cancellationToken: cancellationToken);

                news.NextNews =
                    await _newsRepository.NextNewsAsync(shortLink, news.DateOfRegisration, news.Order, cancellationToken: cancellationToken);

                news.PrevNews =
                    await _newsRepository.PrevNewsAsync(shortLink, news.NextNews != null ? news.NextNews.ShortLink : 0, news.DateOfRegisration, news.Order, cancellationToken: cancellationToken);
            }

            return news;
        }

        [HttpGet]
        [Route("RelatedNews")]
        public async Task<List<NewsSummary>> RelatedNews([FromQuery] int shortLink, CancellationToken cancellationToken) =>
            await _newsRepository.RelatedNewsAsync(shortLink, cancellationToken);


        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.NewsImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }


        [Route("PopularKeywords")]
        [HttpGet]
        public async Task<List<SelectListItem>> PopularKeywords(CancellationToken cancellationToken)
        {
            return
                await _newsRepository.PopularKeywordsAsync(cancellationToken);
        }

        [Route("PagenationKeywords")]
        [HttpPost]
        public async Task<ListActionResult<Keyword>> PagenationKeywords([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _linkRepo.GetAllAsync<Keyword>(query, cancellationToken: cancellationToken);


        [HttpDelete]
        [AccessControl("Admin")]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] Guid id, CancellationToken cancellationToken) =>
             await _mediator.Send(new RemoveNewsCommand(id), cancellationToken);


        [HttpPost]
        [AccessControl("Admin")]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromForm] CreateNewsCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [AccessControl("Admin")]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromForm] UpdateNewsCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

    }
}
