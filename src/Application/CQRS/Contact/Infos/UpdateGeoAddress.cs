using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Infos
{
    public class UpdateGeoAddressCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String IFrame { get; set; }
        public string Route { get; set; }
        public string RouteTitle { get; set; }
        public int Order { get; set; }

    }
}
