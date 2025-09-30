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
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Notices
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly INoticeRepository _noticeRepository;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;
        private readonly IRepository<Link> _linkRepo;

        public NoticeController(
            INoticeRepository noticeRepository,
            IPhotoManager photoManager,
            IWebHostEnvironment hostEnv,
            IMediator mediator,
            IRepository<Link> linkRepo
        )
        {
            _noticeRepository = noticeRepository;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
            _linkRepo = linkRepo;
        }

        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<NoticeSummary>> PagenationSummary(
            [FromBody] GridQuery query,
            CancellationToken cancellationToken
        )
        {
            query.Sorted = new SortModel[]
            {
                new SortModel { column = "dateOfRegisration", desc = true },
                new SortModel { column = "order", desc = false }
            };
            return await _noticeRepository.GetAllAsync<NoticeSummary>(
                query,
                cancellationToken: cancellationToken
            );
        }

        [HttpPost]
        [Route("FilteringPagenation")]
        public async Task<ListActionResult<NoticeSummary>> FilteringPagenation(
            [FromBody] NewsFilteringPagenationQuery query,
            CancellationToken cancellationToken
        )
        {
            return await _noticeRepository.GetAllAsync<NoticeSummary>(
                new GridQuery
                {
                    Page = query.Page,
                    Size = query.Size,
                    Sorted = new SortModel[]
                    {
                        new SortModel { column = "dateOfRegisration", desc = true }
                    }
                },
                b =>
                    (
                        !query.LinksId.Any()
                        || b.Links.Where(b => query.LinksId.Contains(b.LinkId)).Any()
                    )
                    && (
                        String.IsNullOrEmpty(query.Title)
                        || b.Title.Contains(query.Title)
                        || b.Headline.Contains(query.Title)
                        || b.Links.Any(s => s.Link.Value.Contains(query.Title))
                    )
                    &&(
                    b.IsActive == query.IsActive),
                cancellationToken
            );
        }

        [HttpGet]
        [Route("SearchByKeyword")]
        public async Task<ListActionResult<NoticeSummary>> SearchByKeyword(
            [FromQuery] String keyword,
            [FromQuery] int page,
            [FromQuery] int size,
            CancellationToken cancellationToken
        )
        {
            return await _noticeRepository.SearchByKeywordAsync(
                keyword,
                page,
                size,
                cancellationToken
            );
        }

        [HttpGet]
        [Route("Find")]
        public async Task<NoticeDetails?> Find(
            [FromQuery] int shortLink,
            CancellationToken cancellationToken
        )
        {
            await _mediator.Publish(new IncreaseNewsSeenNotification(shortLink));
            var notice = await _noticeRepository.FirstOrDefaultAsync<NoticeDetails>(
                filters: b => b.ShortLink == shortLink,
                cancellationToken: cancellationToken
            );

            if (notice != null)
            {
                notice.RelatedNotices = await _noticeRepository.RelatedNoticesAsync(
                    shortLink,
                    cancellationToken: cancellationToken
                );

                notice.NextNotice = await _noticeRepository.NextNoticeAsync(
                    shortLink,
                    notice.DateOfRegisration,
                    notice.Order,
                    cancellationToken: cancellationToken
                );

                notice.PrevNotice = await _noticeRepository.PrevNoticeAsync(
                    shortLink,
                    notice.NextNotice != null ? notice.NextNotice.ShortLink : 0,
                    notice.DateOfRegisration,
                    notice.Order,
                    cancellationToken: cancellationToken
                );
            }

            return notice;
        }

        [HttpGet]
        [Route("RelatedNews")]
        public async Task<List<NoticeSummary>> RelatedNotices(
            [FromQuery] int shortLink,
            CancellationToken cancellationToken
        ) => await _noticeRepository.RelatedNoticesAsync(shortLink, cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage(
            [FromQuery] ImageQuery query,
            CancellationToken cancellationToken
        )
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.NoticeImagePath}{query.Name}";
            var image = await _photoManager.ResizeAsync(
                path,
                query.Width,
                query.Height,
                cancellationToken
            );

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("PopularKeywords")]
        [HttpGet]
        public async Task<List<SelectListItem>> PopularKeywords(CancellationToken cancellationToken)
        {
            return await _noticeRepository.PopularKeywordsAsync(cancellationToken);
        }

        [Route("PagenationKeywords")]
        [HttpPost]
        public async Task<ListActionResult<Keyword>> PagenationKeywords(
            [FromBody] GridQuery query,
            CancellationToken cancellationToken
        ) => await _linkRepo.GetAllAsync<Keyword>(query, cancellationToken: cancellationToken);

        [HttpDelete]
        [AccessControl(SD.AdminRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(new RemoveNoticeCommand(id), cancellationToken);

        [HttpPost]
        [AccessControl(SD.AdminRole)]
        [Route("Create")]
        public async Task<CommandResponse> Create(
            [FromForm] CreateNoticeCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [AccessControl(SD.AdminRole)]
        [Route("Update")]
        public async Task<CommandResponse> Update(
            [FromForm] UpdateNoticeCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
