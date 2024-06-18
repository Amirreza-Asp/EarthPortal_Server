using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Multimedia.Gallery
{
    public class CreateGalleryCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public IFormFileCollection Images { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
