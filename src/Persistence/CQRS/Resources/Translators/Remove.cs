using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Translators;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Translators
{
    public class RemoveTranslatorCommandHandler
        : IRequestHandler<RemoveTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveTranslatorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveTranslatorCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveTranslatorCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveTranslatorCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.Translator.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (entity == null)
                return CommandResponse.Failure(400, "مترجم مورد نظر در سیستم وجود ندارد");

            _context.Translator.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Translator with id {Id} removed by {UserRealName} in {DoneTime}",
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
