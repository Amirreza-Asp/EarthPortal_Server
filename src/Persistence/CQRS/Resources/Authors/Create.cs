using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Authors;
using Application.Models;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Authors
{
    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateAuthorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateAuthorCommandHandler(ApplicationDbContext context, ILogger<CreateAuthorCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = new Author(request.Name);
            author.Order = request.Order;
            _context.Author.Add(author);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Author with id {author.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(author.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
