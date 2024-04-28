using Application.Models;
using Domain.Entities.Pages;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishCardCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public EnglishCardColor Color { get; set; }
        public String Content { get; set; }
        public EnglishCardType Type { get; set; }
    }
}
