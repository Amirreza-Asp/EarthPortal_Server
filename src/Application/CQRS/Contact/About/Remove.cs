using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.About
{
    public class RemoveAboutCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
