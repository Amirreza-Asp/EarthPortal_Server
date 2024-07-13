using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishPageCommandHandler : IRequestHandler<UpdateEnglishPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateEnglishPageCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateEnglishPageCommandHandler(ApplicationDbContext context, ILogger<UpdateEnglishPageCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateEnglishPageCommand request, CancellationToken cancellationToken)
        {
            var englishPage =
                await _context.EnglishPage
                    .FirstAsync(cancellationToken);

            englishPage.Intro = request.Intro;
            englishPage.CurrentSituation = request.CurrentSituation;
            englishPage.MainIdea = request.MainIdea;
            englishPage.Vision = request.Vision;
            englishPage.Header = request.Header;

            _context.EnglishPage.Update(englishPage);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"EnglishPage updated  by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
