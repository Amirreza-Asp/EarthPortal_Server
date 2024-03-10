using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Resources
{
    public class RemoveBookCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
