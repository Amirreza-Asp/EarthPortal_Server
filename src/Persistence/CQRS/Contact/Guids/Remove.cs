using Application.CQRS.Contact.Guids;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Guids
{
    public class RemoveGuideCommandHandler : IRequestHandler<RemoveGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveGuideCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = await _context.Guide.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (guide == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");


            _context.Guide.Remove(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(guide.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
