using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.EducationalVideos
{
    public class UpdateEducationalVideoCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Video { get; set; }
        public int Order { get; set; }
    }
}
