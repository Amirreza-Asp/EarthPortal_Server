using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Infos
{
    public class AddGeoAddressCommand : IRequest<CommandResponse>
    {
        public String IFrame { get; set; }
        public string RouteTitle { get; set; }
        public string Route { get; set; }
        public int Order { get; set; }
    }
}
