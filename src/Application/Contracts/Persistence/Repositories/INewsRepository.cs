using Application.Models;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;

namespace Application.Contracts.Persistence.Repositories
{
    public interface INewsRepository : IRepository<News>
    {
        Task<List<NewsSummary>> RelatedNewsAsync(int shortLink, CancellationToken cancellationToken);
        Task<ListActionResult<NewsSummary>> SearchByKeywordAsync(String keyword, int page, int size, CancellationToken cancellationToken);
        Task<List<SelectListItem>> PopularKeywordsAsync(CancellationToken cancellationToken);
        Task<NewsSummary?> NextNewsAsync(int shortLink, DateTime dateTime, int order, CancellationToken cancellationToken);
        Task<NewsSummary?> PrevNewsAsync(int shortLink, int shortLink2, DateTime dateTime, int order, CancellationToken cancellationToken);
    }
}
