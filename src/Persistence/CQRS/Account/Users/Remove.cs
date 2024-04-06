using Application.CQRS.Account.User;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account.Users
{
    public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveUserCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<CommandResponse> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var user =
               await _context.User
                   .Where(b => b.UserName == request.UserName)
                   .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return CommandResponse.Failure(400, $"هیچ کاربری با نام کاربری {request.UserName} ثبت نشده است");

            user.IsActive = false;

            _context.User.Update(user);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
