using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.Gallery
{
    public class RemoveGalleryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
