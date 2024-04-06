using Application.Models;
using MediatR;

namespace Application.CQRS.Account.User
{
    public class UpdateUserCommand : IRequest<CommandResponse>
    {
        public String UserName { get; set; }
        public String Name { get; set; }
        public String Family { get; set; }
        public String Password { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public Guid RoleId { get; set; }
    }
}
