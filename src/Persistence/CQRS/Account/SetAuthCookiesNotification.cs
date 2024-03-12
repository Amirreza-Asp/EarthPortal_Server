using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account;
using Application.Utilities;
using Domain;
using Domain.Entities.Account;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account
{

    public class SetAuthCookiesNotificationHandler : INotificationHandler<SetAuthCookiesNotification>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IHttpContextAccessor _contextAccessor;

        public SetAuthCookiesNotificationHandler(IRepository<User> userRepo, IHttpContextAccessor contextAccessor)
        {
            _userRepo = userRepo;
            _contextAccessor = contextAccessor;
        }

        public async Task Handle(SetAuthCookiesNotification notification, CancellationToken cancellationToken)
        {
            var user =
                await _userRepo
                    .FirstOrDefaultAsync(
                        filter: b => b.UserName == notification.UserName,
                        include: source => source.Include(B => B.Role));

            if (user == null)
                return;


            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var token = JWTokenService.GenerateToken(user.UserName, user.Role.Title, ip, user.Email, user.PhoneNumber);


            _contextAccessor.HttpContext.Response.Cookies.Append(SD.AuthToken, ProtectorData.Encrypt(token), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
