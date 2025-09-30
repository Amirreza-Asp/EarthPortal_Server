using Application.Contracts.Persistence.Repositories;
using Application.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class NoticeRepository : Repository<Notice>, INoticeRepository
    {
        public NoticeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<NoticeSummary?> NextNoticeAsync(int shortLink, DateTime dateTime, int order, CancellationToken cancellationToken) =>
            await _context.Notices
                    .Where(b => b.ShortLink != shortLink && b.DateOfRegisration.Date > dateTime.Date && b.Order <= order)
                    .OrderBy(b => b.DateOfRegisration.Date)
                    .ThenBy(b => b.Order)
                    .ProjectTo<NoticeSummary>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);


        public async Task<List<SelectListItem>> PopularKeywordsAsync(CancellationToken cancellationToken)
        {
            return
                await _context.Link
                   .OrderByDescending(b => b.NoticeLinks.Count())
                   .Take(20)
                   .Select(b => new SelectListItem { Text = b.Value, Value = b.Id.ToString() })
                   .ToListAsync(cancellationToken);
        }

        public async Task<NoticeSummary?> PrevNoticeAsync(int shortLink, int shortLink2, DateTime dateTime, int order, CancellationToken cancellationToken) =>
             await _context.Notices
                    .Where(b => b.ShortLink != shortLink && b.ShortLink != shortLink2 && b.DateOfRegisration.Date < dateTime.Date && b.Order >= order)
                    .OrderByDescending(b => b.DateOfRegisration.Date)
                    .ThenBy(b => b.Order)
                    .ProjectTo<NoticeSummary>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<NoticeSummary>> RelatedNoticesAsync(int shortLink, CancellationToken cancellationToken)
        {
            var linksIds =
                await _context.Notices
                .AsNoTracking()
                .Where(b => b.ShortLink == shortLink)
                .Select(b => b.Links.Select(e => e.LinkId))
                .FirstOrDefaultAsync(cancellationToken);

            if (linksIds == null)
                return new List<NoticeSummary>();

            return
                 await _context.Notices
                     .Select(notice => new
                     {
                         Notice = notice,
                         SharedLinksCount = notice.Links.Count(link => linksIds.Any(inputLinkId => inputLinkId == link.LinkId))
                     })
                     .Where(b => b.Notice.ShortLink != shortLink)
                     .OrderByDescending(b => b.SharedLinksCount)
                     .Take(11)
                     .Select(b => b.Notice)
                     .ProjectTo<NoticeSummary>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
        }

        public async Task<ListActionResult<NoticeSummary>> SearchByKeywordAsync(string keyword, int page, int size, CancellationToken cancellationToken)
        {
            var queryContext = _context.Notices.AsQueryable();

            queryContext = queryContext.Where(s => s.Links.Any(l => l.Link.Value == keyword));


            return new ListActionResult<NoticeSummary>
            {
                Data = await queryContext
                        .Skip((page - 1) * size)
                        .Take(size)
                        .OrderByDescending(b => b.DateOfRegisration)
                        .ThenBy(s => s.Order)
                        .ProjectTo<NoticeSummary>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken),
                Total = await queryContext.CountAsync(cancellationToken),
                Size = size,
                Page = page
            };
        }
    }
}
