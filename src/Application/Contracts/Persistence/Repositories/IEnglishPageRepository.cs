using Domain.Dtos.Pages;

namespace Application.Contracts.Persistence.Repositories
{
    public interface IEnglishPageRepository
    {
        Task<EnglishPageDto> GetAsync(CancellationToken cancellationToken = default);

    }
}
