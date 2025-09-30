using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class RemoveNoticeCommand : IRequest<CommandResponse>
    {
        public RemoveNoticeCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

}
