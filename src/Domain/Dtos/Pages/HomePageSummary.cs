using Domain.Dtos.ExternalAPI;
using Domain.Entities.Pages;

namespace Domain.Dtos.Pages
{
    public class HomePageSummary
    {
        public HomePage HomePage { get; set; }
        public CasesAndUsersResponse Cases { get; set; }
    }
}
