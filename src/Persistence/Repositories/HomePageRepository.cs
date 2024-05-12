using Application.Contracts.Persistence.Repositories;
using Application.Contracts.Persistence.Services;
using Application.Models;
using Domain.Dtos.ExternalAPI;
using Domain.Entities.Pages;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class HomePageRepository : IHomePageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IIranelandService _iranelandService;

        public HomePageRepository(ApplicationDbContext context, IIranelandService iranelandService)
        {
            _context = context;
            _iranelandService = iranelandService;
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
            return await _context.HomePage.FirstAsync(cancellationToken);
        }

        public async Task<CommandResponse> UpdateCasesAndUsersAsync(CasesAndUsersResponse model)
        {
            var homePage =
                await _context.HomePage.FirstOrDefaultAsync();

            if (homePage == null)
                return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");

            homePage.Header.ReqCount = model.reqCount;
            homePage.Header.AreaProtectedLandsCount = model.areaProtectedLandsCount;
            homePage.Header.UserCount = model.userCount;

            _context.HomePage.Update(homePage);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
