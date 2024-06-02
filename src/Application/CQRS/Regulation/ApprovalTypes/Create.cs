using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalTypes
{
    public class CreateApprovalTypeCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
