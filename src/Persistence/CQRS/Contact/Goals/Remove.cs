using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Goals;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Goals
{
    public class RemoveGoalCommandHandler : IRequestHandler<RemoveGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveGoalCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveGoalCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveGoalCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveGoalCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.Goal.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (entity == null)
                return CommandResponse.Failure(400, "هدف مورد نظر در سیستم وجود ندارد");

            _context.Goal.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Goal with id {Username} removed by {UserRealName} in {DoneTime}",
                    entity.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
