using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class CreateEnglishCardCommandHandler : IRequestHandler<CreateEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateEnglishCardCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateEnglishCardCommand request, CancellationToken cancellationToken)
        {
            var lastOrderNumber =
                await _context.EnglishCard
                   .Where(b => b.Type == request.Type)
                   .OrderByDescending(b => b.Order)
                   .Select(b => b.Order)
                   .FirstOrDefaultAsync(cancellationToken);

            var englishPageId =
                await _context.EnglishPage
                    .Select(b => b.Id)
                    .FirstAsync(cancellationToken);

            var card = new EnglishCard
            {
                Id = request.Id,
                Color = request.Color,
                Content = request.Content,
                Line = request.Line,
                Order = (byte)(lastOrderNumber + 1),
                Title = request.Title,
                Type = request.Type,
                EnglishPageId = englishPageId,
                SiblingId = request.SiblingId
            };

            _context.EnglishCard.Add(card);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { Order = card.Order });

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
