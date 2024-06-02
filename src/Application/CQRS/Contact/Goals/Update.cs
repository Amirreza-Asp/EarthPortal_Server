using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Goals
{
    public class UpdateGoalCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
