using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.Laws
{
    public class RemoveLawCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
