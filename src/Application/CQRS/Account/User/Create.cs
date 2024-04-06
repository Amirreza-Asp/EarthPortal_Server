using Application.Models;
using MediatR;

namespace Application.CQRS.Account.User
{
    public class CreateUserCommand : IRequest<CommandResponse>
    {
        public String Name { get; set; }
        public String Family { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public Guid RoleId { get; set; }
    }
}
