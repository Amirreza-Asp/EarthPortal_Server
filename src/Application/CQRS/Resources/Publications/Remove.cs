using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Publications
{
    public class RemovePublicationCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
