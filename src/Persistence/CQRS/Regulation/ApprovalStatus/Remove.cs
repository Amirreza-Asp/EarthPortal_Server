using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class RemoveApprovalStatusCommandHandler
        : IRequestHandler<RemoveApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveApprovalStatusCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveApprovalStatusCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveApprovalStatusCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveApprovalStatusCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.ApprovalStatus.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "وضعیت مصوبه انتخاب شده در سیستم وجود ندارد");

            _context.ApprovalStatus.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "ApprovalStatus with id {Id} removed by {UserRealName} in {DoneTime}",
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
