using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Guids;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Guids
{
    public class RemoveGuideCommandHandler : IRequestHandler<RemoveGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveGuideCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveGuideCommandHandler(ApplicationDbContext context, ILogger<RemoveGuideCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = await _context.Guide.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (guide == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");


            _context.Guide.Remove(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Guide with id {guide.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(guide.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
