using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.AboutUsPage
{
    public class UpdateAboutUsPageCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public String Footer { get; set; }
    }
}
