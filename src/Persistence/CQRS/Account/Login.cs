using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account;
using Application.Models;
using Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Account
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, CommandResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordManager _passManager;
        private readonly IMediator _mediator;

        public LoginCommandHandler(IRepository<User> useRepository, IPasswordManager passManager, IMediator mediator)
        {
            _userRepository = useRepository;
            _passManager = passManager;
            _mediator = mediator;
        }

        public async Task<CommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user =
                await _userRepository.FirstOrDefaultAsync(b => b.UserName == request.UserName, include: source => source.Include(b => b.Role));

            if (user == null)
                return CommandResponse.Failure(400, "نام کاربری وارد شده در سیستم وجود ندارد");

            if (!_passManager.VerifyPassword(user.Password, request.Password))
                return CommandResponse.Failure(400, "رمز وارد شده اشتباه است");

            var setCookiesNotification = new SetAuthCookiesNotification(user.UserName);
            await _mediator.Publish(setCookiesNotification);


            return CommandResponse.Success(new { userName = user.UserName, name = user.Name + " " + user.Family, role = user.Role?.Title });
        }
    }
}
