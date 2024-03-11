using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.Goals
{
    public class RemoveGoalCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
