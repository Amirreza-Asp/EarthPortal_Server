using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using AutoMapper;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class CreateEnglishCardCommandHandler
        : IRequestHandler<CreateEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEnglishCardCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateEnglishCardCommandHandler(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<CreateEnglishCardCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateEnglishCardCommand request,
            CancellationToken cancellationToken
        )
        {
            var lastOrderNumber = await _context
                .EnglishCard.Where(b => b.Type == request.Type)
                .OrderByDescending(b => b.Order)
                .Select(b => b.Order)
                .FirstOrDefaultAsync(cancellationToken);

            var englishPageId = await _context
                .EnglishPage.Select(b => b.Id)
                .FirstAsync(cancellationToken);

            var createdCards = new List<EnglishCard>();
            foreach (var item in request.Cards)
            {
                var card = new EnglishCard
                {
                    Id = Guid.NewGuid(),
                    Color = item.Color,
                    Content = item.Content,
                    Line = !createdCards.Any() && request.Line,
                    Order = (byte)(lastOrderNumber + 1 + createdCards.Count),
                    Title = item.Title,
                    Type = request.Type,
                    EnglishPageId = englishPageId,
                };

                createdCards.Add(card);
            }

            if (createdCards.Count > 1)
                (createdCards[0].SiblingId, createdCards[1].SiblingId) = (
                    createdCards[1].Id,
                    createdCards[0].Id
                );

            _context.EnglishCard.AddRange(createdCards);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "New Card added to english page by {UserRealName} in {DoneTime}",
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(_mapper.Map<List<EnglishCardDto>>(createdCards));
            }
            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
