using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Goals
{
    public class CreateGoalCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
    }
}
