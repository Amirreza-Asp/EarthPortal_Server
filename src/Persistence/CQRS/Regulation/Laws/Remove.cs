using Application.CQRS.Regulation.Laws;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.Laws
{
    public class RemoveLawCommandHandler : IRequestHandler<RemoveLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveLawCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveLawCommand request, CancellationToken cancellationToken)
        {
            var law = await _context.Law.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (law == null)
                return CommandResponse.Failure(400, "قانون انتخاب شده در سیستم وجود ندارد");

            _context.Law.Remove(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
