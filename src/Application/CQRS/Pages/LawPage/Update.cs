using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.LawPage
{
    public class UpdateLawPageCommand : IRequest<CommandResponse>
    {
        public String WarningTitle { get; set; }
        public String WarningContent { get; set; }
    }
}
