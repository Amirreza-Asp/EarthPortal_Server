using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Infos
{
    public class RemoveGeoAddressCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
