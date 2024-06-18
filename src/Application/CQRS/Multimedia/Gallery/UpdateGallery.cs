using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Multimedia.Gallery
{
    public class UpdateGalleryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public List<Guid>? DeletedImages { get; set; }
        public List<IFormFile>? Images { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
