using Domain.Dtos.ExternalAPI;

namespace Application.Contracts.Persistence.Services
{
    public interface IIranelandService
    {
        Task<CasesAndUsersResponse> GetCasesAndUsersAsync(CancellationToken cancellationToken = default);
    }


}
