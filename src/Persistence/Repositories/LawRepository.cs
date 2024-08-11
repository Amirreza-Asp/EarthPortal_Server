using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Application.Queries;
using AutoMapper;
using Domain.Dtos.Regulation;
using Domain.Entities.Regulation;
using Microsoft.EntityFrameworkCore;
using Persistence.Utilities;

namespace Persistence.Repositories
{
    public class LawRepository : Repository<Law>, ILawRepository
    {
        public LawRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<ListActionResult<LawSummary>> PaginationSummaryAsync(LawPagenationQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Law> queryContext = _context.Law;



            if (!String.IsNullOrEmpty(query.Text) && query.SearchProps != null && query.SearchProps.Count != 0)
            {
                queryContext =
                    queryContext.Where(b =>
                         query.SearchProps.Contains("title") && b.Title.Contains(query.Text) ||
                         query.SearchProps.Contains("description") && b.Description.Contains(query.Text));
            }

            if (query.LawType != null)
            {
                queryContext = queryContext.Where(b => query.LawType.Contains((int)b.Type));
            }

            if (query.ApprovalAuthorityIds != null && query.ApprovalAuthorityIds.Count != 0)
            {
                queryContext = queryContext.Where(b => query.ApprovalAuthorityIds.Contains(b.ApprovalAuthorityId));
            }

            if (query.ApprovalStatusIds != null && query.ApprovalStatusIds.Count != 0)
            {
                queryContext = queryContext.Where(b => query.ApprovalStatusIds.Contains(b.ApprovalStatusId));
            }

            if (query.ExecutorManagmentIds != null && query.ExecutorManagmentIds.Count != 0)
            {
                queryContext = queryContext.Where(b => query.ExecutorManagmentIds.Contains(b.ExecutorManagmentId));
            }

            if (query.LawCategoryIds != null && query.LawCategoryIds.Count != 0)
            {
                queryContext = queryContext.Where(b => query.LawCategoryIds.Contains(b.LawCategoryId));
            }

            if (query.ApprovalTypeIds != null && query.ApprovalTypeIds.Count != 0)
            {
                queryContext = queryContext.Where(b => query.ApprovalTypeIds.Contains(b.ApprovalTypeId));
            }

            if (query.ApprovalDate.HasValue)
            {
                queryContext = queryContext.Where(b => query.ApprovalDate.Value.Date == b.ApprovalDate.Date);
            }

            if (query.NewspaperDate.HasValue)
            {
                queryContext = queryContext.Where(b => query.NewspaperDate.Value.Date == b.Newspaper.Date.Date);
            }

            if (query.ApprovalDate.HasValue)
            {
                queryContext = queryContext.Where(b => query.AnnouncementDate.Value.Date == b.Announcement.Date);
            }

            if (query.IsOriginal != -1)
            {
                if (query.IsOriginal == 0)
                    queryContext = queryContext.Where(b => b.IsOriginal);
                else
                    queryContext = queryContext.Where(b => !b.IsOriginal);
            }

            queryContext =
                queryContext
                    .Where(
                        law =>
                            String.IsNullOrEmpty(query.AnnouncementNumber) || law.Announcement.Number.Equals(query.AnnouncementNumber) &&
                            String.IsNullOrEmpty(query.NewspaperNumber) || law.Newspaper.Number.Equals(query.NewspaperNumber)
                    );



            var order = GetOrderBy(query);

            queryContext = order(queryContext);

            if (!query.IsFiltered())
            {
                queryContext = queryContext
                    .GroupBy(b => b.Title)
                     .Select(g => g.First());
            }

            var total = await queryContext.CountAsync(cancellationToken);

            var data =
                await queryContext
                    .Skip((query.Page - 1) * query.Size)
                    .Take(query.Size)
                    .ToListAsync(cancellationToken);


            var approvalAuthorities = await _context.ApprovalAuthority.ToListAsync(cancellationToken);
            var approvalStatuses = await _context.ApprovalStatus.ToListAsync(cancellationToken);
            var approvalTypes = await _context.ApprovalType.ToListAsync(cancellationToken);

            foreach (var item in data)
            {
                item.ApprovalType = approvalTypes.Where(b => b.Id == item.ApprovalTypeId).First();
                item.ApprovalStatus = approvalStatuses.Where(b => b.Id == item.ApprovalStatusId).First();
                item.ApprovalAuthority = approvalAuthorities.Where(b => b.Id == item.ApprovalAuthorityId).First();
            }

            var convertedData = _mapper.Map<List<LawSummary>>(data);
            convertedData.ForEach(item => item.ShowArticle = query.IsFiltered());

            return
                new ListActionResult<LawSummary>
                {
                    Data = convertedData,
                    Total = total,
                    Page = query.Page,
                    Size = query.Size
                };

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
