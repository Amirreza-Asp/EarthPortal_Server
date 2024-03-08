using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalStatus
{
    public class UpdateApprovalStatusCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
    }
}
