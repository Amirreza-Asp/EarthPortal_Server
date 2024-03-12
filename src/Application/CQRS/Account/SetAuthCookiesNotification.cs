using MediatR;

namespace Application.CQRS.Account
{
    public class SetAuthCookiesNotification : INotification
    {
        public SetAuthCookiesNotification(string userName)
        {
            UserName = userName;
        }

        public String UserName { get; set; }
    }

}
