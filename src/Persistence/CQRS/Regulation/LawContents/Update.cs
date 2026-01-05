using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.LawContent;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.LawContent
{
    public class UpdateLawContentCommandHandler
        : IRequestHandler<UpdateLawContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateLawContentCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateLawContentCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateLawContentCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateLawContentCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.LawContent.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " موضوع انتخاب شده در سیستم وجود ندارد");

            entity.Title = request.Title;

            _context.LawContent.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    $"LawContent with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
