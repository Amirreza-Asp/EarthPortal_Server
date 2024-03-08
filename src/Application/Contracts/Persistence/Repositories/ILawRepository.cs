using Application.Models;
using Application.Queries;
using Domain.Dtos.Regulation;
using Domain.Entities.Regulation;

namespace Application.Contracts.Persistence.Repositories
{
    public interface ILawRepository : IRepository<Law>
    {
        Task<ListActionResult<LawSummary>> PagenationSummaryAsync(LawPagenationQuery query, CancellationToken cancellationToken);
    }
}
