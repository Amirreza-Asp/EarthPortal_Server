using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.VideoContents
{
    public class CreateVideoContentCommandHandler
        : IRequestHandler<CreateVideoContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateVideoContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateVideoContentCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateVideoContentCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateVideoContentCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var video = new VideoContent(
                request.Title,
                request.Description,
                request.Video,
                request.Link
            );
            video.Order = request.Order;
            video.CreatedAt = request.CreatedAt;
            _context.Add(video);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "VideoContent with id {Username} created by {UserRealName} in {DoneTime}",
                    video.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(video.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
