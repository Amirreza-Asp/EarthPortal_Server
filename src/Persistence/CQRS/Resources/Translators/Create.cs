using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Translators;
using Application.Models;
using Domain.Dtos.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Translators
{
    public class CreateTranslatorCommandHandler : IRequestHandler<CreateTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateTranslatorCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;


        public CreateTranslatorCommandHandler(ApplicationDbContext context, ILogger<CreateTranslatorCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateTranslatorCommand request, CancellationToken cancellationToken)
        {
            var entity = new Translator(request.Name);
            entity.Order = request.Order;
            _context.Translator.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Translator with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
