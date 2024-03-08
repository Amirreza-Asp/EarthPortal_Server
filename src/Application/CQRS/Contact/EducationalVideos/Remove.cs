using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.EducationalVideos
{
    public class RemoveEducationalVideoCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
