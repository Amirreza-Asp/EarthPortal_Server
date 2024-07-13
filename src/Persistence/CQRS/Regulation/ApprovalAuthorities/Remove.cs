using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalAuthorities;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalAuthorities
{
    public class RemoveApprovalAuthorityCommandHandler : IRequestHandler<RemoveApprovalAuthorityCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveApprovalAuthorityCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveApprovalAuthorityCommandHandler(ApplicationDbContext context, ILogger<RemoveApprovalAuthorityCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveApprovalAuthorityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalAuthority.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "مرجع تصویب انتخاب شده در سیستم وجود ندارد");


            _context.ApprovalAuthority.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalAuthority with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
