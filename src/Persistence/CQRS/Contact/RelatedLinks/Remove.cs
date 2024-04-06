using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class RemoveRelatedLinkCommandHandler : IRequestHandler<RemoveRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveRelatedLinkCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<CommandResponse> Handle(RemoveRelatedLinkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.RelatedLink.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "پیوند انتخاب شده در سیستم وجود ندارد");

            _context.RelatedLink.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
