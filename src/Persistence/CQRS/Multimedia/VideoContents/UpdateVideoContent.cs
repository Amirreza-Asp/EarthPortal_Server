﻿using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.VideoContents
{
    public class UpdateVideoContentCommandHandler : IRequestHandler<UpdateVideoContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateVideoContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateVideoContentCommandHandler(ApplicationDbContext context, ILogger<UpdateVideoContentCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateVideoContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var video = await _context.VideoContent.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (video == null)
                return CommandResponse.Failure(400, "ویدیو در سیستم وجود ندارد");

            video.Order = request.Order;
            video.Title = request.Title;
            video.Description = request.Description;
            video.Video = request.Video;
            video.Link = request.Link;
            video.CreatedAt = request.CreatedAt;

            _context.VideoContent.Update(video);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"VideoContent with id {video.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
