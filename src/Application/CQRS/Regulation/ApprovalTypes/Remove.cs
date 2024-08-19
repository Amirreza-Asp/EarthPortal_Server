using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalTypes
{
    public class RemoveApprovalTypeCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
