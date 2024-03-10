using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Guids
{
    public class RemoveGuideCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
