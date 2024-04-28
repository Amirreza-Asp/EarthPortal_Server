using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class RemoveEnglishCardCommandHandler : IRequestHandler<RemoveEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveEnglishCardCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveEnglishCardCommand request, CancellationToken cancellationToken)
        {
            var card =
             await _context.EnglishCard
                .Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (card == null)
                return CommandResponse.Failure(400, "The selected card is not found");

            var otherCards =
                await _context.EnglishCard
                    .Where(b => b.Order > card.Order)
                    .ToListAsync(cancellationToken);

            otherCards.ForEach(c => c.Order = (byte)(c.Order - 1));

            _context.EnglishCard.Remove(card);
            _context.EnglishCard.UpdateRange(otherCards);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
