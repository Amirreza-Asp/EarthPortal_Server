using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalTypes
{
    public class CreateApprovalTypeCommandHandler
        : IRequestHandler<CreateApprovalTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateApprovalTypeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateApprovalTypeCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateApprovalTypeCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateApprovalTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = new Domain.Entities.Regulation.ApprovalType(request.Title);
            entity.Order = request.Order;
            _context.ApprovalType.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "ApprovalType with id {Id} created by {UserRealName} in {DoneTime}",
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
