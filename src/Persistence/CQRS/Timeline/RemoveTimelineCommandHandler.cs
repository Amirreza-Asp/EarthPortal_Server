using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Timeline;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Timeline;

public class RemoveTimelineCommandHandler : IRequestHandler<RemoveTimelineCommand, CommandResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<RemoveTimelineCommandHandler> _logger;
    private readonly IUserAccessor _userAccessor;

    public RemoveTimelineCommandHandler(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        IUserAccessor userAccessor,
        ILogger<RemoveTimelineCommandHandler> logger
    )
    {
        _context = context;
        _env = env;
        _userAccessor = userAccessor;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(
        RemoveTimelineCommand request,
        CancellationToken cancellationToken
    )
    {
        var timeline = await _context.Timeline.FirstOrDefaultAsync(
            b => b.Id == request.Id,
            cancellationToken
        );

        if (timeline == null)
            return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

        var upload = _env.WebRootPath + SD.TimelinePath;

        _context.Timeline.Remove(timeline);

        if (await _context.SaveChangesAsync(cancellationToken) > 0)
        {
            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            if (File.Exists(upload + timeline.Image))
                File.Delete(upload + timeline.Image);

            _logger.LogInformation(
                "Timeline with id {Id} removed by {UserRealName} in {DoneTime}",
                timeline.Id,
                _userAccessor.GetUserName(),
                DateTimeOffset.UtcNow
            );

            return CommandResponse.Success();
        }

        return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
    }
}
