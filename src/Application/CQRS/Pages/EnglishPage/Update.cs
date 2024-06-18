using Application.Models;
using Domain.Entities.Pages;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishPageCommand : IRequest<CommandResponse>
    {
        public EnglishHeader Header { get; set; }

        public EnglishIntro Intro { get; set; }

        public EnglishMainIdea MainIdea { get; set; }

        public EnglishCurrentSituation CurrentSituation { get; set; }

        public EnglishVision Vision { get; set; }
    }
}
