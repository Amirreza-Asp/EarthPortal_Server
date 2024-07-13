using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Guids;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Guids
{
    public class UpdateGuideCommandHandler : IRequestHandler<UpdateGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateGuideCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateGuideCommandHandler(ApplicationDbContext context, ILogger<UpdateGuideCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = await _context.Guide.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (guide == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            guide.Title = request.Title;
            guide.IsPort = request.IsPort.ToLower() == "true";
            guide.Content = request.Content;
            guide.Order = request.Order;

            _context.Guide.Update(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Guide with id {guide.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(guide.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
