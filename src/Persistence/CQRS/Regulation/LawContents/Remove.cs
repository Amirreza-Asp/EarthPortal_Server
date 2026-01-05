using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.LawContent;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.LawContent
{
    public class RemoveLawContentCommandHandler
        : IRequestHandler<RemoveLawContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveLawContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveLawContentCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveLawContentCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveLawContentCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.LawContent.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " موضوع انتخاب شده در سیستم وجود ندارد");

            _context.LawContent.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    $"LawContent with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
