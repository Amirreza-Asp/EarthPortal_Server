using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class CreateApprovalStatusCommandHandler : IRequestHandler<CreateApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateApprovalStatusCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;


        public CreateApprovalStatusCommandHandler(ApplicationDbContext context, ILogger<CreateApprovalStatusCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateApprovalStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.Regulation.ApprovalStatus(request.Title);
            entity.Order = request.Order;
            _context.ApprovalStatus.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ApprovalStatus with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
