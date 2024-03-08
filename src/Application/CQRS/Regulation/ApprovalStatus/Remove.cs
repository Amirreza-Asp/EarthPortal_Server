using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalStatus
{
    public class RemoveApprovalStatusCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
