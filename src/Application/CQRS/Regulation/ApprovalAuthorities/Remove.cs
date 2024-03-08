using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalAuthorities
{
    public class RemoveApprovalAuthorityCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
