using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageRemoveSolutionCommandHandler
        : IRequestHandler<EnglishPageRemoveSolutionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnglishPageRemoveSolutionCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public EnglishPageRemoveSolutionCommandHandler(
            ApplicationDbContext context,
            ILogger<EnglishPageRemoveSolutionCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            EnglishPageRemoveSolutionCommand request,
            CancellationToken cancellationToken
        )
        {
            var solution = await _context
                .EnglishPageSolution.Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (solution == null)
                return CommandResponse.Failure(400, "The operation failed");

            _context.EnglishPageSolution.Remove(solution);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Solution with id {Id} removed from english page by {UserRealName} in {DoneTime}",
                    request.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
