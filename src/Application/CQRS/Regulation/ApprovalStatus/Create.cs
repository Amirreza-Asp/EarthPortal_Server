using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ApprovalStatus
{
    public class CreateApprovalStatusCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
