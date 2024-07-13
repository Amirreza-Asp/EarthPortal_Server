using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Guids;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Guids
{
    public class CreateGuideCommandHandler : IRequestHandler<CreateGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateGuideCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateGuideCommandHandler(ApplicationDbContext context, ILogger<CreateGuideCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = new Guide(request.Title, request.Content, request.IsPort.ToLower() == "true");
            guide.Order = request.Order;
            _context.Guide.Add(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Guide with id {guide.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(guide.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
