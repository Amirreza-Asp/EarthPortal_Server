using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.FAQ
{
    public class CreateFAQCommand : IRequest<CommandResponse>
    {

        public String Title { get; set; }
        public String Description { get; set; }
        public int Order { get; set; }
    }
}
