using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class UpdateApprovalStatusCommandHandler : IRequestHandler<UpdateApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateApprovalStatusCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateApprovalStatusCommandHandler(ApplicationDbContext context, ILogger<UpdateApprovalStatusCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateApprovalStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalStatus.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " وضعیت مصوبه انتخاب شده در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Status = request.Title;

            _context.ApprovalStatus.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalStatus with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
