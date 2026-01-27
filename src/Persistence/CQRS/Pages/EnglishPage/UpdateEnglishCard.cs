using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishCardCommandHandler
        : IRequestHandler<UpdateEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateEnglishCardCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateEnglishCardCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateEnglishCardCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateEnglishCardCommand request,
            CancellationToken cancellationToken
        )
        {
            var card = await _context
                .EnglishCard.Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (card == null)
                return CommandResponse.Failure(400, "The selected card is not found");

            card.Content = request.Content;
            card.Title = request.Title;

            _context.EnglishCard.Update(card);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Card with id {Id} updated from EnglishPage by {UserRealName} in {DoneTime}",
                    card.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
