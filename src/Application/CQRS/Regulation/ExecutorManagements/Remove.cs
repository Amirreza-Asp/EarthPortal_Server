using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ExecutorManagements
{
    public class RemoveExecutorManagementCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
