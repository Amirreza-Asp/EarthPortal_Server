using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.VideoContents
{
    public class UpdateVideoContentCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Video { get; set; }
    }
}
