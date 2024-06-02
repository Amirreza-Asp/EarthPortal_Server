using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Guids
{
    public class CreateGuideCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public String IsPort { get; set; }
        public int Order { get; set; }
    }
}
