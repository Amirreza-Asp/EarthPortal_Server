using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Account.User;
using Application.Models;
using Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordManager _passManager;
        public CreateUserCommandHandler(ApplicationDbContext context, IPasswordManager passManager)
        {
            _context = context;
            _passManager = passManager;
        }

        public async Task<CommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!_passManager.CheckCharacters(request.UserName).Valid)
                return CommandResponse.Failure(400, "نام کاربری فقط باید شامل A-Z یا a-z یا کاراکتر های (! , * , @ , #) باشد");

            var validatePassword = _passManager.CheckPasswordStrong(request.Password);
            if (!validatePassword.Valid)
                return CommandResponse.Failure(400, validatePassword.Error);

            if (await _context.User.AnyAsync(b => b.UserName == request.UserName))
                return CommandResponse.Failure(400, "نام کاربری وارد شده در سیستم وجود دارد");

            var user = new User(request.RoleId, request.Name, request.Family, request.UserName, _passManager.HashPassword(request.Password), request.Email, request.PhoneNumber);
            _context.User.Add(user);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(user.CreatedAt);

            return CommandResponse.Failure(400, "عملیات با خطا مواجه شد");

        }
    }
}
