using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Authors;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Authors
{
    public class RemoveAuthorCommandHandler : IRequestHandler<RemoveAuthorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveAuthorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveAuthorCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveAuthorCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveAuthorCommand request,
            CancellationToken cancellationToken
        )
        {
            var author = await _context.Author.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (author == null)
                return CommandResponse.Failure(400, "نویسنده مورد نظر در سیستم وجود ندارد");

            _context.Author.Remove(author);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Author with id {Id} removed by {UserRealName} in {DoneTime}",
                    author.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
