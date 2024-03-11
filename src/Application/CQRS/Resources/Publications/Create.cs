using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Publications
{
    public class CreatePublicationCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
    }
}
