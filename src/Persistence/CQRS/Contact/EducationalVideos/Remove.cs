using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.EducationalVideos
{
    public class RemoveEducationalVideoCommandHandler
        : IRequestHandler<RemoveEducationalVideoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveEducationalVideoCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveEducationalVideoCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveEducationalVideoCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveEducationalVideoCommand request,
            CancellationToken cancellationToken
        )
        {
            var edv = await _context.EducationalVideo.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (edv == null)
                return CommandResponse.Failure(400, "ویدیو مورد نظر در سیستم وجود ندارد");

            _context.EducationalVideo.Remove(edv);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "EducationalVideo with id {Username} removed by {UserRealName} in {DoneTime}",
                    edv.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
