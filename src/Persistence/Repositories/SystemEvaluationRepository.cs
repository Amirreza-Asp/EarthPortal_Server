using Application.Contracts.Persistence.Repositories;
using Application.Utilities;
using Domain.Dtos.Contact;
using Domain.Entities.Contact.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class SystemEvaluationRepository : ISystemEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemEvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SystemEvaluationDetails> GetAsync(CancellationToken cancellationToken)
        {
            var model = new SystemEvaluationDetails();

            model.TotalVote = await _context.SystemEvaluation.CountAsync();

            model.Votes =
                await _context.SystemEvaluation
                    .GroupBy(b => b.Score)
                    .Select(b => new Vote { Count = b.Count(), Score = b.Key })
                    .ToListAsync(cancellationToken);

            for (int i = 1; i <= 5; i++)
                if (!model.Votes.Select(b => b.Score).Contains(i))
                    model.Votes.Add(new Vote { Score = i, Count = 0 });

            model.Votes = model.Votes.OrderByDescending(b => b.Score).ToList();

            model.IntroductionMethods =
                await _context.IntroductionMethod
                    .GroupBy(b => b.IntroductionMethod)
                    .Select(b => new IntrodMethod { Method = EnumHelper.GetEnumDisplayName(b.Key), Count = b.Count() })
                    .ToListAsync(cancellationToken);

            foreach (var item in Enum.GetValues(typeof(IntroductionMethod)))
            {
                var itemEnumType = (IntroductionMethod)item;
                var itemDisplay = EnumHelper.GetEnumDisplayName(itemEnumType);

                if (!model.IntroductionMethods.Select(b => b.Method).Contains(itemDisplay))
                    model.IntroductionMethods.Add(new IntrodMethod { Method = itemDisplay, Count = 0 });
            }

            model.IntroductionMethods = model.IntroductionMethods.OrderByDescending(b => b.Count).ToList();

            model.Pages =
                await _context.SystemEvaluationPage
                    .GroupBy(b => b.Page)
                    .Select(b => new UsefulPage { Page = EnumHelper.GetEnumDisplayName(b.Key), Count = b.Count() })
                    .ToListAsync(cancellationToken);

            foreach (var item in Enum.GetValues(typeof(Pages)))
            {
                var itemEnumType = (Pages)item;
                var itemDisplay = EnumHelper.GetEnumDisplayName(itemEnumType);

                if (!model.Pages.Select(b => b.Page).Contains(itemDisplay))
                    model.Pages.Add(new UsefulPage { Page = itemDisplay, Count = 0 });
            }

            model.Pages = model.Pages.OrderByDescending(b => b.Count).ToList();

            return model;
        }
    }
}
