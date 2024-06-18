using Application.Contracts.Persistence.Repositories;
using AutoMapper;
using Domain.Dtos.Pages;
using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class EnglishPageRepository : IEnglishPageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EnglishPageRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EnglishPageDto> GetAsync(CancellationToken cancellationToken = default)
        {
            var data =
                await _context.EnglishPage
                    .Include(b => b.Cards)
                    .Include(b => b.Solutions)
                    .Include(b => b.Problems)
                    .FirstAsync(cancellationToken);

            return new EnglishPageDto
            {
                Id = data.Id,
                CurrentSituation = data.CurrentSituation,
                MainIdea = data.MainIdea,
                Intro = data.Intro,
                Header = data.Header,
                Problems = data.Problems.Select(b => new EnglishItem { Id = b.Id, Content = b.Content }).ToList(),
                Vision = data.Vision,
                Solutions = data.Solutions.Select(b => new EnglishItem { Id = b.Id, Content = b.Content }).ToList(),
                BeginningCards = SortCards(data.Cards, EnglishCardType.Beginning),
                MiddleCards = SortCards(data.Cards, EnglishCardType.Middle),
                EndCards = SortCards(data.Cards, EnglishCardType.End)
            };
        }

        private List<EnglishCardContainerDto> SortCards(List<EnglishCard> cards, EnglishCardType type)
        {
            cards = cards.Where(b => b.Type == type).OrderBy(b => b.Order).ToList();

            var data = new List<EnglishCardContainerDto>();

            foreach (var card in cards)
            {
                if (!data.Any(b => b.Cards.Any(b => b.Id == card.Id)))
                {
                    if (card.SiblingId == default)
                    {
                        data.Add(
                            new EnglishCardContainerDto
                            {
                                Cards = new List<EnglishCardDto> { _mapper.Map<EnglishCardDto>(card) }
                            });
                    }
                    else
                    {
                        var sibling = cards.Where(b => b.Id == card.SiblingId).First();

                        data.Add(
                            new EnglishCardContainerDto
                            {
                                Cards = new List<EnglishCardDto> {
                                            _mapper.Map<EnglishCardDto>(card),
                                            _mapper.Map<EnglishCardDto>(sibling)
                                }.OrderBy(b => b.Order).ToList()
                            });
                    }
                }
            }

            return data;
        }
    }
}
