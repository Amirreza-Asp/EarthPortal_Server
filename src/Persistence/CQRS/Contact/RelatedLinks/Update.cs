using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class UpdateRelatedLinkCommandHandler
        : IRequestHandler<UpdateRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateRelatedLinkCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateRelatedLinkCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateRelatedLinkCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateRelatedLinkCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.RelatedLink.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (entity == null)
                return CommandResponse.Failure(400, "پیوند انتخاب شده در سیستم وجود ندارد");

            entity.Title = request.Title;
            entity.Link = request.Link;
            entity.Order = request.Order;

            _context.RelatedLink.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "RelatedLink with id {Username} updated by {UserRealName} in {DoneTime}",
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
