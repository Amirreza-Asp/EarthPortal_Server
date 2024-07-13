using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Translators;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Translators
{
    public class UpdateTranslatorCommandHandler : IRequestHandler<UpdateTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateTranslatorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;


        public UpdateTranslatorCommandHandler(ApplicationDbContext context, ILogger<UpdateTranslatorCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateTranslatorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Translator.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "مترجم مورد نظر در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.FullName = request.Name;

            _context.Translator.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Translator with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
