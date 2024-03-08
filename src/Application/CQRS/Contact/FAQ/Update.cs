using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.FAQ
{
    public class UpdateFAQCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
    }
}
