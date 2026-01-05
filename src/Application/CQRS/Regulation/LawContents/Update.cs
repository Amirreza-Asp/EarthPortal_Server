using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.LawContent
{
    public class UpdateLawContentCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
    }
}
