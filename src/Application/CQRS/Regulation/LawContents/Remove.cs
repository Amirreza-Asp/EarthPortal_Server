using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.LawContent
{
    public class RemoveLawContentCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
