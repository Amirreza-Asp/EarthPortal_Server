using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Goals;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Goals
{
    public class CreateGoalCommandHandler : IRequestHandler<CreateGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateGoalCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateGoalCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateGoalCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateGoalCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = new Goal(request.Title);
            entity.Order = request.Order;

            _context.Goal.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Goal with id {Username} created by {UserRealName} in {DoneTime}",
                    entity.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
