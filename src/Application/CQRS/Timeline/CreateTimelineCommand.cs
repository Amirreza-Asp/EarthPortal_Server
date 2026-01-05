using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.CQRS.Timeline;

public class CreateTimelineCommand : IRequest<CommandResponse>
{
    [Required]
    public required string Title { get; set; }

    [Required]
    [StringLength(500, ErrorMessage = "بیشتر از 500 کاراکتر نمیتواند باشد")]
    public required string Content { get; set; }
    public IFormFile? Image { get; set; }
    public string? Video { get; set; }
    public bool IsVideo { get; set; }
    public DateTime AccomplishedDate { get; set; }
}
