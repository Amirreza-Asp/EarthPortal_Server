using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.CommonicationWays
{
    public class UpdateInfoCommand : IRequest<CommandResponse>
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AparatLink { get; set; }
        public string BaleLink { get; set; }
        public string RubikaLink { get; set; }
        public string EitaaLink { get; set; }
        public string Gap { get; set; }
        public string IGap { get; set; }
    }
}
