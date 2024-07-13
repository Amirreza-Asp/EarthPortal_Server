using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalTypes
{
    public class UpdateApprovalTypeCommandHandler : IRequestHandler<UpdateApprovalTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateApprovalTypeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateApprovalTypeCommandHandler(ApplicationDbContext context, ILogger<UpdateApprovalTypeCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateApprovalTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalType.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " نوع مصوبه انتخاب شده در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Value = request.Title;

            _context.ApprovalType.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalType with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
