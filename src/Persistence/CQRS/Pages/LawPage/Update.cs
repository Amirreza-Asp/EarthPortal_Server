using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.LawPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.LawPage
{
    public class UpdateLawPageCommandHandler : IRequestHandler<UpdateLawPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateLawPageCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateLawPageCommandHandler(ApplicationDbContext context, ILogger<UpdateLawPageCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateLawPageCommand request, CancellationToken cancellationToken)
        {
            var entity =
                await _context.LawPage
                    .FirstAsync(cancellationToken);

            entity.WarningTitle = request.WarningTitle;
            entity.WarningContent = request.WarningContent;

            _context.LawPage.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"LawPage content updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
