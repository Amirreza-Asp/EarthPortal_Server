using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Info : BaseEntity
    {
        public Info(string phoneNumber, string email, string aparatLink, string baleLink, string rubikaLink, string eitaaLink)
        {
            PhoneNumber = phoneNumber;
            Email = email;
            AparatLink = aparatLink;
            BaleLink = baleLink;
            RubikaLink = rubikaLink;
            EitaaLink = eitaaLink;
        }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AparatLink { get; set; }
        public string BaleLink { get; set; }
        public string RubikaLink { get; set; }
        public string EitaaLink { get; set; }

        public ICollection<GeoAddress> GeoAddresses { get; set; }
    }


}
