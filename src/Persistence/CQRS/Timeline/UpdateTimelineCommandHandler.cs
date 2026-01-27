using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Timeline;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Timeline;

public class UpdateTimelineCommandHandler : IRequestHandler<UpdateTimelineCommand, CommandResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPhotoManager _photoManager;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UpdateTimelineCommandHandler> _logger;
    private readonly IUserAccessor _userAccessor;

    public UpdateTimelineCommandHandler(
        ApplicationDbContext context,
        IPhotoManager photoManager,
        IWebHostEnvironment env,
        ILogger<UpdateTimelineCommandHandler> logger,
        IUserAccessor userAccessor
    )
    {
        _context = context;
        _photoManager = photoManager;
        _env = env;
        _logger = logger;
        _userAccessor = userAccessor;
    }

    public async Task<CommandResponse> Handle(
        UpdateTimelineCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            //if (request.IsVideo && String.IsNullOrEmpty(request.Video))
            //    return CommandResponse.Failure(400, "ویدیو را وارد کنید");

            //if (request.IsVideo && !request.Video.Contains("iframe"))
            //    return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نمی باشد");

            var timeline = await _context.Timeline.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (timeline == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            timeline.Title = request.Title;
            timeline.Content = request.Content;
            timeline.AccomplishedDate = request.AccomplishedDate;
            var upload = _env.WebRootPath + SD.TimelinePath;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            if (request.Video != null || request.Image != null)
            {
                if (!request.IsVideo)
                {
                    var oldImage = timeline.Image;
                    if (request.Image != null)
                        timeline.Image = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

                    timeline.Video = null;
                    _context.Timeline.Update(timeline);

                    if (request.Image != null)
                        _photoManager.Save(request.Image, upload + timeline.Image);

                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        if (
                            request.Image != null
                            && oldImage != null
                            && File.Exists(upload + oldImage)
                        )
                            File.Delete(upload + oldImage);

                        _logger.LogInformation(
                            "Timeline with id {Id} updated by {UserRealName} in {DoneTime}",
                            timeline.Id,
                            _userAccessor.GetUserName(),
                            DateTimeOffset.UtcNow
                        );

                        return CommandResponse.Success(new { Image = timeline.Image });
                    }
                }
                else
                {
                    var oldImage = timeline.Image;
                    timeline.Video = request.Video;
                    timeline.Image = null;

                    _context.Timeline.Update(timeline);

                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        if (oldImage != null && File.Exists(upload + oldImage))
                            File.Delete(upload + oldImage);

                        _logger.LogInformation(
                            "Timeline with id {Id} updated by {UserRealName} in {DoneTime}",
                            timeline.Id,
                            _userAccessor.GetUserName(),
                            DateTimeOffset.UtcNow
                        );

                        return CommandResponse.Success();
                    }
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
