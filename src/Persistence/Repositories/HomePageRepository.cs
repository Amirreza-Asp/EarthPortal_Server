using Application.Contracts.Persistence.Repositories;
using Application.Models;
using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class HomePageRepository : IHomePageRepository
    {
        private readonly ApplicationDbContext _context;

        public HomePageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> ChangeHeaderAsync(HomeHeader header, CancellationToken cancellationToken)
        {
            var page = await _context.HomePage.FirstAsync(cancellationToken);

            page.Header = header;

            _context.HomePage.Update(page);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }

        public async Task<CommandResponse> ChangeWorkAsync(HomeWork work, CancellationToken cancellationToken)
        {
            var page = await _context.HomePage.FirstAsync(cancellationToken);

            page.Work = work;

            _context.HomePage.Update(page);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }

        public async Task<HomePage> GetAsync(CancellationToken cancellationToken)
        {
            var data = await _context.HomePage.ToListAsync(cancellationToken);
            return data.First();
        }

    }
}
