using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Authors;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Authors
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateAuthorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateAuthorCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateAuthorCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateAuthorCommand request,
            CancellationToken cancellationToken
        )
        {
            var author = await _context.Author.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (author == null)
                return CommandResponse.Failure(400, "نویسنده مورد نظر در سیستم وجود ندارد");

            author.Order = request.Order;
            author.FullName = request.Name;

            _context.Author.Update(author);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Author with id {Id} updated by {UserRealName} in {DoneTime}",
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
