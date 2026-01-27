using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Publications;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Publications
{
    public class UpdatePublicationCommandHandler
        : IRequestHandler<UpdatePublicationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePublicationCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdatePublicationCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdatePublicationCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdatePublicationCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.Publication.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (entity == null)
                return CommandResponse.Failure(400, "انتشارات مورد نظر در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Title = request.Title;

            _context.Publication.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Publication with id {Id} updated by {UserRealName} in {DoneTime}",
                    entity.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
