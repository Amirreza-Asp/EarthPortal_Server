using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.EducationalVideos
{
    public class CreateEducationalVideoCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Video { get; set; }
        public int Order { get; set; }
    }
}
