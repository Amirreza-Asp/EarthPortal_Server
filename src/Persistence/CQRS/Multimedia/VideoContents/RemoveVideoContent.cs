using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.VideoContents
{
    public class RemoveVideoContentCommandHandler : IRequestHandler<RemoveVideoContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveVideoContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveVideoContentCommandHandler(ApplicationDbContext context, ILogger<RemoveVideoContentCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveVideoContentCommand request, CancellationToken cancellationToken)
        {
            var video = await _context.VideoContent.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (video == null)
                return CommandResponse.Failure(400, "ویدیو در سیستم وجود ندارد");

            _context.VideoContent.Remove(video);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"VideoContent with id {video.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
