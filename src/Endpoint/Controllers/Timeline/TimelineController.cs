using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Timeline;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Timeline;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Timeline;

[Route("api/[controller]")]
[ApiController]
public class TimelineController : ControllerBase
{
    private readonly IRepository<Domain.Entities.Timelines.Timeline> _timelineRepo;
    private readonly IMediator _mediator;
    private readonly IPhotoManager _photoManager;
    private readonly IWebHostEnvironment _hostEnv;

    public TimelineController(
        IRepository<Domain.Entities.Timelines.Timeline> timeline,
        IMediator mediator,
        IPhotoManager photoManager,
        IWebHostEnvironment hostEnv
    )
    {
        _timelineRepo = timeline;
        _mediator = mediator;
        _photoManager = photoManager;
        _hostEnv = hostEnv;
    }

    [HttpPost]
    [Route("PaginationSummary")]
    public async Task<ListActionResult<TimelineSummary>> PaginationSummary(
        [FromBody] GridQuery query,
        CancellationToken cancellationToken
    ) =>
        await _timelineRepo.GetAllAsync<TimelineSummary>(
            query,
            cancellationToken: cancellationToken
        );

    [Route("Image")]
    [HttpGet]
    public async Task<IActionResult> GetImage(
        [FromQuery] ImageQuery query,
        CancellationToken cancellationToken
    )
    {
        string upload = _hostEnv.WebRootPath;
        string path = $"{upload}{SD.TimelinePath}{query.Name}";
        var image = await _photoManager.ResizeAsync(
            path,
            query.Width,
            query.Height,
            cancellationToken
        );

        string extension = Path.GetExtension(query.Name);
        return File(image, $"image/{extension.Substring(1)}");
    }

    [Route("Create")]
    [HttpPost]
    [AccessControl(SD.AdminRole)]
    public async Task<CommandResponse> Create(
        [FromForm] CreateTimelineCommand command,
        CancellationToken cancellationToken
    ) => await _mediator.HandleRequestAsync(command, cancellationToken);

    [Route("Update")]
    [HttpPut]
    [AccessControl(SD.AdminRole)]
    public async Task<CommandResponse> Update(
        [FromForm] UpdateTimelineCommand command,
        CancellationToken cancellationToken
    ) => await _mediator.HandleRequestAsync(command, cancellationToken);

    [Route("Remove")]
    [HttpDelete]
    [AccessControl(SD.AdminRole)]
    public async Task<CommandResponse> Remove(
        [FromQuery] RemoveTimelineCommand command,
        CancellationToken cancellationToken
    ) => await _mediator.HandleRequestAsync(command, cancellationToken);
}
