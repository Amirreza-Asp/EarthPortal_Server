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

        public ICollection<GeoAddressSummary> GeoAddresses { get; set; }
    }

    public class GeoAddressSummary
    {
        public Guid Id { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public List<double> Cordinates { get; set; }
        public string Route { get; set; }
    }
}
