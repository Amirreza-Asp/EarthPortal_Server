using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account;
using Application.Models;
using Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Account
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, CommandResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordManager _passManager;
        private readonly IMediator _mediator;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IRepository<User> useRepository, IPasswordManager passManager, IMediator mediator, ILogger<LoginCommandHandler> logger)
        {
            _userRepository = useRepository;
            _passManager = passManager;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (!await _userRepository.AnyAsync(b => b.UserName == request.UserName))
                return CommandResponse.Failure(400, "نام کاربری وارد شده در سیستم وجود ندارد");

            var user =
                await _userRepository.FirstOrDefaultAsync(b => b.UserName == request.UserName, include: source => source.Include(b => b.Role));

            if (user == null)
                return CommandResponse.Failure(400, "نام کاربری وارد شده در سیستم وجود ندارد");

            if (!_passManager.VerifyPassword(user.Password, request.Password))
                return CommandResponse.Failure(400, "رمز وارد شده اشتباه است");

            var setCookiesNotification = new SetAuthCookiesNotification(user.UserName);
            await _mediator.Publish(setCookiesNotification);

            _logger.LogInformation($"User with username : {request.UserName} logged in {DateTime.Now}");

            return CommandResponse.Success(new { userName = user.UserName, name = user.Name + " " + user.Family, role = user.Role?.Title, roleDisplay = user.Role?.Display });
        }
    }
}
