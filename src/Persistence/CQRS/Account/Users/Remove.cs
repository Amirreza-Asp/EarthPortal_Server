using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Account.User;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Account.Users
{
    public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveUserCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveUserCommandHandler(ApplicationDbContext context, ILogger<RemoveUserCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
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
            {
                _logger.LogInformation($"user with username {request.UserName} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
