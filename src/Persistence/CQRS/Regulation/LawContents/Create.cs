using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.LawContent;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.LawContent
{
    public class CreateLawContentCommandHandler
        : IRequestHandler<CreateLawContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateLawContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateLawContentCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateLawContentCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateLawContentCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = new Domain.Entities.Regulation.LawContent(request.Title);
            _context.LawContent.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    $"LawContent with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
