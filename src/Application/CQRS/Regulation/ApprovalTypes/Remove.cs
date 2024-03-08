using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalTypes
{
    public class RemoveApprovalTypeCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
