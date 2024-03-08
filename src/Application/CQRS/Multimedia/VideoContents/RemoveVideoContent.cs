using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.VideoContents
{
    public class RemoveVideoContentCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
