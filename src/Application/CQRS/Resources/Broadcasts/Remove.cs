using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Broadcasts
{
    public class RemoveBroadcastCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
