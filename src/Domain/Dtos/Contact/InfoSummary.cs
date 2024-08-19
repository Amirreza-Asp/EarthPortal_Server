namespace Domain.Dtos.Contact
{
    public class InfoSummary
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AparatLink { get; set; }
        public string BaleLink { get; set; }
        public string RubikaLink { get; set; }
        public string EitaaLink { get; set; }
        public string GapLink { get; set; }
        public string IGapLink { get; set; }

        public ICollection<GeoAddressSummary> GeoAddresses { get; set; }
    }

    public class GeoAddressSummary
    {
        public Guid Id { get; set; }
        public string Route { get; set; }
        public String IFrame { get; set; }
        public string RouteTitle { get; set; }
        public int Order { get; set; }
    }
}
