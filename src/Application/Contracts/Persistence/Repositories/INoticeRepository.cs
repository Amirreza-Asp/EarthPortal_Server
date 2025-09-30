using Application.Models;
using Domain.Dtos.Notices;
using Domain.Dtos.Shared;
using Domain.Entities.Notices;

namespace Application.Contracts.Persistence.Repositories
{
    public interface INoticeRepository : IRepository<Notice>
    {
        Task<List<NoticeSummary>> RelatedNoticesAsync(int shortLink, CancellationToken cancellationToken);
        Task<ListActionResult<NoticeSummary>> SearchByKeywordAsync(String keyword, int page, int size, CancellationToken cancellationToken);
        Task<List<SelectListItem>> PopularKeywordsAsync(CancellationToken cancellationToken);
        Task<NoticeSummary?> NextNoticeAsync(int shortLink, DateTime dateTime, int order, CancellationToken cancellationToken);
        Task<NoticeSummary?> PrevNoticeAsync(int shortLink, int shortLink2, DateTime dateTime, int order, CancellationToken cancellationToken);
    }
}
