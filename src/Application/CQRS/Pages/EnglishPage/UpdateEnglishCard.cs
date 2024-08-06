using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishCardCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
    }
}
