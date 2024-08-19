using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalAuthorities
{
    public class UpdateApprovalAuthorityCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
