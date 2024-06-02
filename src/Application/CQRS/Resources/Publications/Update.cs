using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Publications
{
    public class UpdatePublicationCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public String Title { get; set; }
    }
}
