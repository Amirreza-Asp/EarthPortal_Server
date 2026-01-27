using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class RemoveEnglishCardCommandHandler
        : IRequestHandler<RemoveEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveEnglishCardCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveEnglishCardCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveEnglishCardCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveEnglishCardCommand request,
            CancellationToken cancellationToken
        )
        {
            var card = await _context
                .EnglishCard.Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (card == null)
                return CommandResponse.Failure(400, "The selected card is not found");

            var otherCards = await _context
                .EnglishCard.Where(b => b.Order > card.Order)
                .ToListAsync(cancellationToken);

            otherCards.ForEach(c => c.Order = (byte)(c.Order - 1));

            var sibling = await _context
                .EnglishCard.Where(b => b.SiblingId == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (sibling != null)
            {
                sibling.SiblingId = null;
                _context.EnglishCard.Update(sibling);
            }

            _context.EnglishCard.Remove(card);
            _context.EnglishCard.UpdateRange(otherCards);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Card with id {Id} removed from english page by {UserRealName} in {DoneTime}",
                    request.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
