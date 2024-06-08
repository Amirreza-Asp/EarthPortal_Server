using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.Gallery
{
    public class SetPhotoOrderCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public List<GalleryPhotoOrder> Images { get; set; }
    }

    public class GalleryPhotoOrder
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
    }
}
