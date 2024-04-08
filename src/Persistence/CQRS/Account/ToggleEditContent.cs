using Application;
using Application.CQRS.Account;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account
{
    public class ToggleEditContentCommandHandler : IRequestHandler<ToggleEditContentCommand, CommandResponse>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ApplicationDbContext _context;

        public ToggleEditContentCommandHandler(IHttpContextAccessor accessor, ApplicationDbContext context)
        {
            _accessor = accessor;
            _context = context;
        }

        public async Task<CommandResponse> Handle(ToggleEditContentCommand request, CancellationToken cancellationToken)
        {
            var userName = _accessor.HttpContext.User.Claims.First(b => b.Type == AppClaims.UserName).Value;

            var user =
                await _context.User.FirstOrDefaultAsync(b => b.UserName == userName, cancellationToken);

            if (user == null)
                return CommandResponse.Failure(400, "کاربر در سیستم وجود ندارد");

            user.EnableContentEdit = !user.EnableContentEdit;

            _context.User.Update(user);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");

        }
    }
}
