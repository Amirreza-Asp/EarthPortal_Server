using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Timeline;

public class UpdateTimelineCommand : IRequest<CommandResponse>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public IFormFile? Image { get; set; }
    public string? Video { get; set; }
    public bool IsVideo { get; set; }
    public DateTime AccomplishedDate { get; set; }
}


