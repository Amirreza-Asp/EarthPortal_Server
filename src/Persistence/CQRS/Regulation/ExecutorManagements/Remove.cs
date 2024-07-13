using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class RemoveExecutorManagementCommandHandler : IRequestHandler<RemoveExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveExecutorManagementCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveExecutorManagementCommandHandler(ApplicationDbContext context, ILogger<RemoveExecutorManagementCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ExecutorManagment.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "دستگاه مجری انتخاب شده در سیستم وجود ندارد");


            _context.ExecutorManagment.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"ExecutorManagement with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
