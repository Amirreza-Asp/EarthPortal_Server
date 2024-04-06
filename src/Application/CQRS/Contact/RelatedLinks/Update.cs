using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.RelatedLinks
{
    public class UpdateRelatedLinkCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Link { get; set; }
    }
}
