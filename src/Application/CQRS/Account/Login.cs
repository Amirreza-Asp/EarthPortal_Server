using Application.Models;
using MediatR;

namespace Application.CQRS.Account
{
    public class LoginCommand : IRequest<CommandResponse>
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Captcha { get; set; }

    }


}
