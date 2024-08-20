using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using AutoMapper;
using Domain.Dtos.Regulation;
using Domain.Entities.Regulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Persistence.Repositories
{
    public class LawRepository : Repository<Law>, ILawRepository
    {
        private readonly IMemoryCache _memoryCache;
        public LawRepository(ApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache) : base(context, mapper)
        {
            _memoryCache = memoryCache;
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await _context.Law.Select(s => s.Title).Distinct().CountAsync(cancellationToken);
        }

        public async Task<DateTime?> GetLastModifiedAsync(CancellationToken cancellationToken)
        {
            return await _context.Law.OrderByDescending(b => b.LastModifiedAt).Select(b => b.LastModifiedAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ListActionResult<LawSummary>> PaginationSummaryAsync(LawPagenationQuery query, CancellationToken cancellationToken)
        {
            if (!_memoryCache.TryGetValue("laws", out List<Law>? laws))
            {
                laws =
                    await _context.Law
                        .Include(b => b.ApprovalAuthority)
                        .Include(b => b.ApprovalType)
                        .Include(b => b.ApprovalStatus)
                        .Include(b => b.ExecutorManagment)
                        .Include(b => b.LawCategory)
                        .ToListAsync(cancellationToken);

                _memoryCache.Set("laws", laws, DateTimeOffset.Now + TimeSpan.FromHours(1));
            }

            laws = SearchByQuery(laws, query.SearchProps, query.Text);

            laws = laws?.Where(b => query.LawType == null || query.LawType.Contains((int)b.Type)).ToList();

            laws = FilterBySpecification(laws, query.ApprovalAuthorityIds, b => query.ApprovalAuthorityIds.Contains(b.ApprovalAuthorityId));
            laws = FilterBySpecification(laws, query.ApprovalStatusIds, b => query.ApprovalStatusIds.Contains(b.ApprovalStatusId));
            laws = FilterBySpecification(laws, query.ExecutorManagmentIds, b => query.ExecutorManagmentIds.Contains(b.ExecutorManagmentId));
            laws = FilterBySpecification(laws, query.LawCategoryIds, b => query.LawCategoryIds.Contains(b.LawCategoryId));
            laws = FilterBySpecification(laws, query.ApprovalTypeIds, b => query.ApprovalTypeIds.Contains(b.ApprovalTypeId));


            if (query.ApprovalDate.HasValue)
            {
                laws = laws?.Where(b => query.ApprovalDate.Value.Date == b.ApprovalDate.Date).ToList();
            }

            if (query.NewspaperDate.HasValue)
            {
                laws = laws?.Where(b => query.NewspaperDate.Value.Date == b.Newspaper?.Date.Date).ToList();
            }

            if (query.ApprovalDate.HasValue)
            {
                laws = laws?.Where(b => query.AnnouncementDate.Value.Date == b.Announcement?.Date.Date).ToList();
            }

            if (query.IsOriginal != -1)
            {
                if (query.IsOriginal == 0)
                    laws = laws?.Where(b => b.IsOriginal).ToList();
                else
                    laws = laws?.Where(b => !b.IsOriginal).ToList();
            }

            laws =
                laws?
                    .Where(
                        law =>
                            String.IsNullOrEmpty(query.AnnouncementNumber) || law.Announcement?.Number == query.AnnouncementNumber &&
                            String.IsNullOrEmpty(query.NewspaperNumber) || law.Newspaper?.Number == query.NewspaperNumber
                    )
                    .ToList();

            if (!query.IsFiltered())
            {
                laws = laws?.DistinctBy(b => b.Title).ToList();
            }

            laws = GetOrderBy(laws, query.SortBy, query.ascSort);

            var total = laws?.Count();

            var data =
               laws?
                    .Skip((query.Page - 1) * query.Size)
                    .Take(query.Size)
                    .ToList();

            var convertedData = _mapper.Map<List<LawSummary>>(data);
            convertedData.ForEach(item => item.ShowArticle = query.IsFiltered());

            return
                new ListActionResult<LawSummary>
                {
                    Data = convertedData,
                    Total = total.HasValue ? total.Value : 0,
                    Page = query.Page,
                    Size = query.Size
                };
        }

        public List<Law>? SearchByQuery(List<Law>? laws, List<String> searchProps, String? query)
        {
            return laws?.Where(b =>
                           String.IsNullOrEmpty(query) ||
                           (searchProps.Contains("title") && b.Title.Contains(query)) ||
                           (searchProps.Contains("description") && b.Description.Contains(query)))
                        .ToList();
        }

        public List<Law>? FilterBySpecification(List<Law>? laws, List<Guid>? filterItems, Func<Law, bool> filter)
        {
            if (filterItems != null && filterItems.Count > 0)
                laws = laws?.Where(filter).ToList();

            return laws;
        }

        private List<Law>? GetOrderBy(List<Law>? laws, string? sortBy, bool asc)
        {
            Func<Law, object?> orderBySelector = sortBy?.ToLower() switch
            {
                "approvalauthority" => law => law.ApprovalAuthority?.Name,
                "title" => law => law.Title,
                "regulation" => law => law.Id,
                "status" => law => law.ApprovalStatus.Status,
                _ => law => law.ApprovalDate
            };

            return asc
                ? laws?.OrderBy(orderBySelector).ToList()
                : laws?.OrderByDescending(orderBySelector).ToList();
        }

    }
}
