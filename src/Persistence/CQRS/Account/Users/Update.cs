using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Account.User;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordManager _passManager;

        public UpdateUserCommandHandler(ApplicationDbContext context, IPasswordManager passManager)
        {
            _context = context;
            _passManager = passManager;
        }

        public async Task<CommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user =
                await _context.User
                    .Where(b => b.UserName == request.UserName)
                    .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return CommandResponse.Failure(400, $"هیچ کاربری با نام کاربری {request.UserName} ثبت نشده است");

            if (request.Password != "TestPassword")
            {
                var validatePassword = _passManager.CheckPasswordStrong(request.Password);
                if (!validatePassword.Valid)
                    return CommandResponse.Failure(400, validatePassword.Error);

                user.Password = _passManager.HashPassword(request.Password);
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Family = request.Family;
            user.RoleId = request.RoleId;
            user.PhoneNumber = request.PhoneNumber;

            _context.User.Update(user);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
