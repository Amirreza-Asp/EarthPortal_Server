using Application.Models;
using Domain.Dtos.ExternalAPI;
using Domain.Entities.Pages;

namespace Application.Contracts.Persistence.Repositories
{
    public interface IHomePageRepository
    {
        Task<HomePage> GetAsync(CancellationToken cancellationToken);

        Task<CommandResponse> ChangeHeaderAsync(HomeHeaderDto header, CancellationToken cancellationToken);
        Task<CommandResponse> ChangeWorkAsync(HomeWork work, CancellationToken cancellationToken);
        Task<CommandResponse> UpdateCasesAndUsersAsync(CasesAndUsersResponse model);



    }

    public class HomeHeaderDto
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public bool PortBtnEnable { get; set; }
        public bool AppBtnEnable { get; set; }
    }
}
