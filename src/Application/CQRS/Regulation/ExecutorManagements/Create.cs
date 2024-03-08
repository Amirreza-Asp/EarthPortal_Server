using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.ExecutorManagements
{
    public class CreateExecutorManagementCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }
    }
}
