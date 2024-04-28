using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishCardCommandHandler : IRequestHandler<UpdateEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateEnglishCardCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateEnglishCardCommand request, CancellationToken cancellationToken)
        {
            var card =
                await _context.EnglishCard
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (card == null)
                return CommandResponse.Failure(400, "The selected card is not found");

            card.Content = request.Content;
            card.Title = request.Title;
            card.Color = request.Color;
            card.Type = request.Type;

            _context.EnglishCard.Update(card);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");

        }
    }
}
