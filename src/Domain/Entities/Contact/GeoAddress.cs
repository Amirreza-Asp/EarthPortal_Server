using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class GeoAddress : BaseEntity
    {
        public GeoAddress(String iFrame, string route, Guid infoId, string routeTitle, int order) : base(Guid.NewGuid())
        {
            IFrame = iFrame;
            Route = route;
            InfoId = infoId;
            RouteTitle = routeTitle;
            Order = order;
        }

        private GeoAddress() { }

        public string IFrame { get; set; }
        public String RouteTitle { get; set; }
        public string Route { get; set; }
        public Guid InfoId { get; set; }


        public Info Info { get; set; }
    }
}
