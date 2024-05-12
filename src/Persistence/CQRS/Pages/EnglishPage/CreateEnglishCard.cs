using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using AutoMapper;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class CreateEnglishCardCommandHandler : IRequestHandler<CreateEnglishCardCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateEnglishCardCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreateEnglishCardCommand request, CancellationToken cancellationToken)
        {
            var lastOrderNumber =
                await _context.EnglishCard
                   .Where(b => b.Type == request.Cards[0].Type)
                   .OrderByDescending(b => b.Order)
                   .Select(b => b.Order)
                   .FirstOrDefaultAsync(cancellationToken);

            var englishPageId =
                await _context.EnglishPage
                    .Select(b => b.Id)
                    .FirstAsync(cancellationToken);

            var data = new List<EnglishCard>();

            foreach (var item in request.Cards)
            {
                var card = new EnglishCard
                {
                    Id = item.Id,
                    Color = item.Color,
                    Content = item.Content,
                    Line = item.Line,
                    Order = (byte)(lastOrderNumber + 1),
                    Title = item.Title,
                    Type = item.Type,
                    EnglishPageId = englishPageId,
                    SiblingId = item.SiblingId
                };

                data.Add(card);
                _context.EnglishCard.Add(card);
            }


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { data = _mapper.Map<List<EnglishCardDto>>(data) });

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
