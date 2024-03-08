using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;

namespace Application.Contracts.Persistence.Repositories
{
    public interface INewsRepository : IRepository<News>
    {
        Task<List<NewsSummary>> RelatedNewsAsync(int shortLink, CancellationToken cancellationToken);
        Task<List<SelectListItem>> PopularKeywordsAsync(CancellationToken cancellationToken);
    }
}
