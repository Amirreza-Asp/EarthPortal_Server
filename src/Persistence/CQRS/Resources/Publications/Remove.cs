using Application.CQRS.Resources.Publications;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Publications
{
    public class RemovePublicationCommandHandler : IRequestHandler<RemovePublicationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemovePublicationCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemovePublicationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Publication.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "انتشارات مورد نظر در سیستم وجود ندارد");

            _context.Publication.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
