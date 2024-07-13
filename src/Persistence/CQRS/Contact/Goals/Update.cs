using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Goals;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Goals
{
    public class UpdateGoalCommandHandler : IRequestHandler<UpdateGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateGoalCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateGoalCommandHandler(ApplicationDbContext context, ILogger<UpdateGoalCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Goal.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "هدف مورد نظر در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Title = request.Title;

            _context.Goal.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Goal with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
