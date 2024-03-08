using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.FAQ
{
    public class RemoveFAQCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
