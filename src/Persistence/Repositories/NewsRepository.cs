using Application.Contracts.Persistence.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<NewsSummary?> NextNewsAsync(int shortLink, DateTime dateTime, CancellationToken cancellationToken) =>
            await _context.News
                    .Where(b => b.ShortLink != shortLink && b.DateOfRegisration.Date > dateTime.Date)
                    .OrderBy(b => b.DateOfRegisration.Date)
                    .ProjectTo<NewsSummary>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);


        public async Task<List<SelectListItem>> PopularKeywordsAsync(CancellationToken cancellationToken)
        {
            return
                await _context.Link
                   .OrderByDescending(b => b.NewsLinks.Count())
                   .Take(20)
                   .Select(b => new SelectListItem { Text = b.Value, Value = b.Id.ToString() })
                   .ToListAsync(cancellationToken);
        }

        public async Task<NewsSummary?> PrevNewsAsync(int shortLink, int shortLink2, DateTime dateTime, CancellationToken cancellationToken) =>
             await _context.News
                    .Where(b => b.ShortLink != shortLink && b.ShortLink != shortLink2 && b.DateOfRegisration.Date < dateTime.Date)
                    .OrderByDescending(b => b.DateOfRegisration.Date)
                    .ProjectTo<NewsSummary>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<NewsSummary>> RelatedNewsAsync(int shortLink, CancellationToken cancellationToken)
        {
            var linksIds =
                await _context.News
                .AsNoTracking()
                .Where(b => b.ShortLink == shortLink)
                .Select(b => b.Links.Select(e => e.LinkId))
                .FirstOrDefaultAsync(cancellationToken);

            if (linksIds == null)
                return new List<NewsSummary>();

            return
                 await _context.News
                     .Select(news => new
                     {
                         News = news,
                         SharedLinksCount = news.Links.Count(link => linksIds.Any(inputLinkId => inputLinkId == link.LinkId))
                     })
                     .Where(b => b.News.ShortLink != shortLink)
                     .OrderByDescending(b => b.SharedLinksCount)
                     .Take(11)
                     .Select(b => b.News)
                     .ProjectTo<NewsSummary>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
        }
    }
}
