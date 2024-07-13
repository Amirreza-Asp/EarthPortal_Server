using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalTypes
{
    public class RemoveApprovalTypeCommandHandler : IRequestHandler<RemoveApprovalTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveApprovalTypeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveApprovalTypeCommandHandler(ApplicationDbContext context, ILogger<RemoveApprovalTypeCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveApprovalTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalType.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " نوع مصوبه انتخاب شده در سیستم وجود ندارد");


            _context.ApprovalType.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalType with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
