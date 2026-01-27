using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.Contracts.Persistence.Services;
using Application.Models;
using Domain.Dtos.ExternalAPI;
using Domain.Entities.Pages;
using Domain.Entities.Timelines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class HomePageRepository : IHomePageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomePageRepository> _logger;
        private readonly IUserAccessor _userAccessor;
        private readonly IIranelandService _iranelandService;
        private readonly IMemoryCache _memoryCache;
        private static string _cacheKey = "HomePageKey";

        public HomePageRepository(
            ApplicationDbContext context,
            IIranelandService iranelandService,
            ILogger<HomePageRepository> logger,
            IUserAccessor userAccessor,
            IMemoryCache memoryCache
        )
        {
            _context = context;
            _iranelandService = iranelandService;
            _logger = logger;
            _userAccessor = userAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<CommandResponse> ChangeHeaderAsync(
            HomeHeaderDto header,
            CancellationToken cancellationToken
        )
        {
            var page = await _context.HomePage.FirstAsync(cancellationToken);

            page.Header.Title = header.Title;
            page.Header.Content = header.Content;
            page.Header.AppBtnEnable = header.AppBtnEnable;
            page.Header.PortBtnEnable = header.PortBtnEnable;

            _context.HomePage.Update(page);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _memoryCache.Remove(_cacheKey);

                _logger.LogInformation(
                    "HomePage Updated by {UserRealName} in {DoneTime}",
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }

        public async Task<CommandResponse> ChangeWorkAsync(
            HomeWork work,
            CancellationToken cancellationToken
        )
        {
            var page = await _context.HomePage.FirstAsync(cancellationToken);

            page.Work = work;

            _context.HomePage.Update(page);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _memoryCache.Remove(_cacheKey);

                _logger.LogInformation(
                    "HomePage Updated by {UserRealName} in {DoneTime}",
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }

        public async Task<HomePage> GetAsync(CancellationToken cancellationToken)
        {
            if (!_memoryCache.TryGetValue(_cacheKey, out HomePage data))
            {
                data = await _context.HomePage.FirstAsync(cancellationToken);
                ;

                _memoryCache.Set(_cacheKey, data);
            }

            return data;
        }

        public async Task<CommandResponse> UpdateCasesAndUsersAsync(CasesAndUsersResponse model)
        {
            var homePage = await _context.HomePage.FirstOrDefaultAsync();

            if (homePage == null)
                return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");

            if (model != null)
            {
                homePage.Header.ReqCount = Convert.ToInt32(model.reqCount);
                homePage.Header.AreaProtectedLandsCount = Convert.ToInt32(
                    model.areaProtectedLandsCount
                );
                homePage.Header.UserCount = Convert.ToInt32(model.userCount);

                _context.HomePage.Update(homePage);

                if (await _context.SaveChangesAsync() > 0)
                {
                    _memoryCache.Remove(_cacheKey);

                    _logger.LogInformation(
                        "HomePage Updated by {UserRealName} in {DoneTime}",
                        _userAccessor.GetUserName(),
                        DateTimeOffset.UtcNow
                    );

                    return CommandResponse.Success();
                }
            }
            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
