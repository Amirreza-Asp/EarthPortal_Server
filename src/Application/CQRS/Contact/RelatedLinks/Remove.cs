using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.RelatedLinks
{
    public class RemoveRelatedLinkCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
