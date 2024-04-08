using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Infos
{
    public class AddGeoAddressCommand : IRequest<CommandResponse>
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string RouteTitle { get; set; }
        public string Route { get; set; }
    }
}
