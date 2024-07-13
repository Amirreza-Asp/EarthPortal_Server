using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalAuthorities;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalAuthorities
{
    public class UpdateApprovalAuthorityCommandHandler : IRequestHandler<UpdateApprovalAuthorityCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateApprovalAuthorityCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateApprovalAuthorityCommandHandler(ApplicationDbContext context, ILogger<UpdateApprovalAuthorityCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateApprovalAuthorityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalAuthority.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "مرجع تصویب انتخاب شده در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Name = request.Title;

            _context.ApprovalAuthority.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalAuthority with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
