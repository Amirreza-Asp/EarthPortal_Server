using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalAuthorities
{
    public class CreateApprovalAuthorityCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
    }
}
