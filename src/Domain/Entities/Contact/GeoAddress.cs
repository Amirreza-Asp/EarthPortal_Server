using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class GeoAddress : BaseEntity
    {
        public GeoAddress(double lat, double lon, string route, Guid infoId, string routeTitle)
        {
            Lat = lat;
            Lon = lon;
            Route = route;
            InfoId = infoId;
            RouteTitle = routeTitle;
        }

        public double Lat { get; set; }
        public double Lon { get; set; }
        public String RouteTitle { get; set; }
        public string Route { get; set; }
        public Guid InfoId { get; set; }

        public Info Info { get; set; }
    }
}
