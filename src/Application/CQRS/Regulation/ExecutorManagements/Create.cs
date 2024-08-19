using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ExecutorManagements
{
    public class CreateExecutorManagementCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public string Title { get; set; }
        public int Order { get; set; }
    }
}
