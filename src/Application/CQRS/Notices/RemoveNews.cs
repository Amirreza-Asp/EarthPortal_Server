using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class RemoveNewsCommand : IRequest<CommandResponse>
    {
        public RemoveNewsCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

}
