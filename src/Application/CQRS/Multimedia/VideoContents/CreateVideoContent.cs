using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.VideoContents
{
    public class CreateVideoContentCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public string Description { get; set; }
        public String Video { get; set; }
    }
}
