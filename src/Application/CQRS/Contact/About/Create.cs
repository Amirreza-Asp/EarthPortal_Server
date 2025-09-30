using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Contact.About
{
    public class CreateAboutCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? Video { get; set; }
        public bool IsVideo { get; set; }
        public DateTime AccomplishedDate { get; set; }
        public int Order { get; set; }
    }
}
