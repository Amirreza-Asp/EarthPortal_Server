using Application.Models;
using Domain.Entities.Pages;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class CreateEnglishCardCommand : IRequest<CommandResponse>
    {
        public bool Line { get; set; }
        public EnglishCardType Type { get; set; }

        public List<CreateEnglishCardData> Cards { get; set; }
    }

    public class CreateEnglishCardData
    {
        public String Title { get; set; }
        public EnglishCardColor Color { get; set; }
        public String Content { get; set; }
    }
}
