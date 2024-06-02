using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Publications
{
    public class CreatePublicationCommand : IRequest<CommandResponse>
    {
        public int Order { get; set; }
        public String Title { get; set; }
    }
}
