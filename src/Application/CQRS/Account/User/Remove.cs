using Application.Models;
using MediatR;

namespace Application.CQRS.Account.User
{
    public class RemoveUserCommand : IRequest<CommandResponse>
    {
        public String UserName { get; set; }
    }
}
