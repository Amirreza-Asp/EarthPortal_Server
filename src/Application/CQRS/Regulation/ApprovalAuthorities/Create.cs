using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalAuthorities
{
    public class CreateApprovalAuthorityCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
