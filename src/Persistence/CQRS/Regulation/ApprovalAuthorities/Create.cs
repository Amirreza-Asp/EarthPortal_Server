using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalAuthorities;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalAuthorities
{
    public class CreateApprovalAuthorityCommandHandler
        : IRequestHandler<CreateApprovalAuthorityCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<CreateApprovalAuthorityCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateApprovalAuthorityCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateApprovalAuthorityCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateApprovalAuthorityCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = new ApprovalAuthority(request.Title);
            entity.Order = request.Order;
            _context.ApprovalAuthority.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "ApprovalAuthority with id {Id} updated by {UserRealName} in {DoneTime}",
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
