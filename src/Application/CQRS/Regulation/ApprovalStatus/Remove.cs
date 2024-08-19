using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalStatus
{
    public class RemoveApprovalStatusCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
