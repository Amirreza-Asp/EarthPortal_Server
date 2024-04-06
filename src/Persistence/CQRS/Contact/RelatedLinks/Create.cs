using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class CreateRelatedLinkCommandHandler : IRequestHandler<CreateRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateRelatedLinkCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateRelatedLinkCommand request, CancellationToken cancellationToken)
        {
            var entity = new RelatedLink(request.Title, request.Link);

            _context.RelatedLink.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
