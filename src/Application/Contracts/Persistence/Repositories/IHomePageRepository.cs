using Application.Models;
using Domain.Entities.Pages;

namespace Application.Contracts.Persistence.Repositories
{
    public interface IHomePageRepository
    {
        Task<HomePage> GetAsync(CancellationToken cancellationToken);

        Task<CommandResponse> ChangeHeaderAsync(HomeHeader header, CancellationToken cancellationToken);
        Task<CommandResponse> ChangeWorkAsync(HomeWork work, CancellationToken cancellationToken);
    }
}
