using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.LawContent
{
    public class CreateLawContentCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public String Title { get; set; }
    }
}
