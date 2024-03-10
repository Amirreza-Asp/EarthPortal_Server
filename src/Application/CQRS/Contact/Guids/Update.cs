using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Guids
{
    public class UpdateGuideCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String IsPort { get; set; }
    }
}
