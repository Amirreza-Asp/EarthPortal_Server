using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class UpdateExecutorManagementCommandHandler : IRequestHandler<UpdateExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateExecutorManagementCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateExecutorManagementCommandHandler(ApplicationDbContext context, ILogger<UpdateExecutorManagementCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ExecutorManagment.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "دستگاه مجری انتخاب شده در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Name = request.Title;

            _context.ExecutorManagment.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"ExecutorManagement with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
