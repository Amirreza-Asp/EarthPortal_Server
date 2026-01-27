using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Publications;
using Application.Models;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Publications
{
    public class CreatePublicationCommandHandler
        : IRequestHandler<CreatePublicationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatePublicationCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreatePublicationCommandHandler(
            ApplicationDbContext context,
            ILogger<CreatePublicationCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreatePublicationCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = new Publication(request.Title);

            entity.Order = request.Order;
            _context.Publication.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Publication with id {Id} created by {UserRealName} in {DoneTime}",
                    entity.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
