using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Timeline;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Timeline;

public class CreateTimelineCommandHandler : IRequestHandler<CreateTimelineCommand, CommandResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IPhotoManager _photoManager;
    private readonly ILogger<CreateTimelineCommandHandler> _logger;
    private readonly IUserAccessor _userAccessor;

    public CreateTimelineCommandHandler(
        ApplicationDbContext context,
        IPhotoManager photoManager,
        IWebHostEnvironment env,
        IUserAccessor userAccessor,
        ILogger<CreateTimelineCommandHandler> logger
    )
    {
        _context = context;
        _photoManager = photoManager;
        _env = env;
        _userAccessor = userAccessor;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(
        CreateTimelineCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            //if (request.IsVideo && String.IsNullOrEmpty(request.Video))
            //    return CommandResponse.Failure(400, "ویدیو را وارد کنید");

            //if (request.IsVideo && !request.Video.Contains("iframe"))
            //    return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نمی باشد");

            //if (!request.IsVideo && request.Image == null)
            //    return CommandResponse.Failure(400, "عکس را وارد کنید");

            if (request.Image == null || request.Video == null)
            {
                var timeline = new Domain.Entities.Timelines.Timeline(
                    request.Title,
                    request.Content,
                    null,
                    null,
                    request.AccomplishedDate
                );
                _context.Timeline.Add(timeline);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {
                    _logger.LogInformation(
                        "Timeline with id {Id} registered by {UserRealName} in {DoneTime}",
                        timeline.Id,
                        _userAccessor.GetUserName(),
                        DateTimeOffset.UtcNow
                    );

                    return CommandResponse.Success(new { id = timeline.Id });
                }
            }
            else if (!request.IsVideo)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                var upload = _env.WebRootPath + SD.TimelinePath;

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                _photoManager.Save(request.Image, upload + imgName);

                var timeline = new Domain.Entities.Timelines.Timeline(
                    request.Title,
                    request.Content,
                    null,
                    imgName,
                    request.AccomplishedDate
                );
                _context.Timeline.Add(timeline);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {
                    _logger.LogInformation(
                        "Timeline with id {Id} registered by {UserRealName} in {DoneTime}",
                        timeline.Id,
                        _userAccessor.GetUserName(),
                        DateTimeOffset.UtcNow
                    );

                    return CommandResponse.Success(new { id = timeline.Id, image = imgName });
                }
            }
            else
            {
                var timeline = new Domain.Entities.Timelines.Timeline(
                    request.Title,
                    request.Content,
                    request.Video,
                    null,
                    request.AccomplishedDate
                );
                _context.Timeline.Add(timeline);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {
                    _logger.LogInformation(
                        "Timeline with id {Id} registered by {UserRealName} in {DoneTime}",
                        timeline.Id,
                        _userAccessor.GetUserName(),
                        DateTimeOffset.UtcNow
                    );

                    return CommandResponse.Success(new { id = timeline.Id });
                }
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
        catch (Exception ex)
        {
            return CommandResponse.Failure(500, ex);
        }
    }
}
