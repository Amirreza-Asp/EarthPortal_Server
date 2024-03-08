using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ExecutorManagements
{
    public class RemoveExecutorManagementCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
