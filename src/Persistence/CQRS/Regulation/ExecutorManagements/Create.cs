using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class CreateExecutorManagementCommandHandler : IRequestHandler<CreateExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateExecutorManagementCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateExecutorManagementCommandHandler(ApplicationDbContext context, ILogger<CreateExecutorManagementCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = new ExecutorManagment(request.Title);
            entity.Order = request.Order;
            _context.ExecutorManagment.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"ExecutorManagement with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
