using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Notices;
using Application.Models;
using Domain.Entities.Notices;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Notices
{
    public class RemoveNoticeCommandHandler : IRequestHandler<RemoveNoticeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RemoveNoticeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveNoticeCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<RemoveNoticeCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveNoticeCommand request,
            CancellationToken cancellationToken
        )
        {
            var notice = await _context.Notices.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (notice == null)
                return CommandResponse.Failure(400, "اطلاعیه مورد نظر در سیستم وجود ندارد");

            _context.Notices.Remove(notice);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Notice with id {Username} removed by {UserRealName} in {DoneTime}",
                    notice.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "حذف با شکست مواجه شد");
        }
    }
}
