using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalAuthorities
{
    public class RemoveApprovalAuthorityCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
