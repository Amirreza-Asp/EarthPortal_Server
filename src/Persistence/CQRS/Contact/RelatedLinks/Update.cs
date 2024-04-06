using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class UpdateRelatedLinkCommandHandler : IRequestHandler<UpdateRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateRelatedLinkCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateRelatedLinkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.RelatedLink.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "پیوند انتخاب شده در سیستم وجود ندارد");

            entity.Title = request.Title;
            entity.Link = request.Link;

            _context.RelatedLink.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
