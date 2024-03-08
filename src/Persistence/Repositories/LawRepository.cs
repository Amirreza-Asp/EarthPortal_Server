using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Dtos.Regulation;
using Domain.Entities.Regulation;
using Microsoft.EntityFrameworkCore;
using Persistence.Utilities;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class LawRepository : Repository<Law>, ILawRepository
    {
        public LawRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<ListActionResult<LawSummary>> PagenationSummaryAsync(LawPagenationQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Law> queryContext = _context.Law;

            var searchFilter = GetSearchFilter(query);
            var lawTypeFilter = GetLawTypeFilter(query);
            var relationsFilter = GetRelationsFilter(query);
            var datesFilter = GetDatesFilter(query);

            queryContext =
                queryContext
                    .Where(searchFilter)
                    .Where(lawTypeFilter)
                    .Where(relationsFilter)
                    .Where(datesFilter)
                    .Where(
                        law =>
                            query.IsOriginal == -1 ? true : query.IsOriginal == 0 ? !law.IsOriginal : law.IsOriginal &&
                            String.IsNullOrEmpty(query.AnnouncementNumber) || law.Announcement.Number.Equals(query.AnnouncementNumber) &&
                            String.IsNullOrEmpty(query.NewspaperNumber) || law.Newspaper.Number.Equals(query.NewspaperNumber)
                    );

            var total = await queryContext.CountAsync(cancellationToken);

            var order = GetOrderBy(query);

            queryContext = order(queryContext);

            var data =
                await queryContext
                    .Skip((query.Page - 1) * query.Size)
                    .Take(query.Size)
                    .ProjectTo<LawSummary>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

            return
                new ListActionResult<LawSummary>
                {
                    Data = data,
                    Total = total,
                    Page = query.Page,
                    Size = query.Size
                };
        }



        private Expression<Func<Law, bool>> GetSearchFilter(LawPagenationQuery query)
        {
            return b => String.IsNullOrEmpty(query.Text) ? true :
                        query.SearchProps == null || !query.SearchProps.Any() ? false :
                         query.SearchProps.Contains("title") && b.Title.Contains(query.Text) ||
                         query.SearchProps.Contains("description") && b.Description.Contains(query.Text);
        }
        private Expression<Func<Law, bool>> GetLawTypeFilter(LawPagenationQuery query)
        {
            return b => query.LawType == null ? false : query.LawType.Contains((int)b.Type);
        }
        private Expression<Func<Law, bool>> GetRelationsFilter(LawPagenationQuery query)
        {
            return
                law =>
                    !query.ApprovalAuthorityIds.Any() ? true : query.ApprovalAuthorityIds.Contains(law.ApprovalAuthorityId) &&
                    !query.ApprovalStatusIds.Any() ? true : query.ApprovalStatusIds.Contains(law.ApprovalStatusId) &&
                    !query.ExecutorManagmentIds.Any() ? true : query.ExecutorManagmentIds.Contains(law.ExecutorManagmentId) &&
                    !query.LawCategoryIds.Any() ? true : query.LawCategoryIds.Contains(law.LawCategoryId) &&
                    !query.ApprovalTypeIds.Any() ? true : query.ApprovalTypeIds.Contains(law.ApprovalTypeId);
        }
        private Expression<Func<Law, bool>> GetDatesFilter(LawPagenationQuery query)
        {
            return
                law =>
                    (!query.ApprovalDate.HasValue || query.ApprovalDate == null || query.ApprovalDate.Value.Date == law.ApprovalDate.Date) &&
                    (!query.NewspaperDate.HasValue || query.NewspaperDate == null || query.NewspaperDate.Value.Date == law.Newspaper.Date.Date) &&
                    (!query.AnnouncementDate.HasValue || query.AnnouncementDate == null || query.AnnouncementDate.Value.Date == law.Announcement.Date);
        }
        private Func<IQueryable<Law>, IOrderedQueryable<Law>> GetOrderBy(LawPagenationQuery query)
        {
            switch (query.SortBy)
            {
                case "approvaldate": return b => b.DynamicOrderBy(law => law.ApprovalDate, query.ascSort);
                case "approvalauthority": return b => b.DynamicOrderBy(law => law.ApprovalAuthority.Name, query.ascSort);
                case "title": return b => b.DynamicOrderBy(law => law.Title, query.ascSort);
                case "regulation": return b => b.DynamicOrderBy(law => law.Id, query.ascSort);
                case "status": return b => b.DynamicOrderBy(law => law.ApprovalStatus.Status, query.ascSort);
            }

            return b => b.DynamicOrderBy(law => law.ApprovalDate, query.ascSort);
        }

    }
}
