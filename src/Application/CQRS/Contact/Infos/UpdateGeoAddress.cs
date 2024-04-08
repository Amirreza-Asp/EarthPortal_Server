using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Infos
{
    public class UpdateGeoAddressCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Route { get; set; }
        public string RouteTitle { get; set; }

    }
}
