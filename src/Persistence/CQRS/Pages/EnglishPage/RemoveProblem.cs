using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageRemoveProblemCommandHandler : IRequestHandler<EnglishPageRemoveProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnglishPageRemoveProblemCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public EnglishPageRemoveProblemCommandHandler(ApplicationDbContext context, ILogger<EnglishPageRemoveProblemCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(EnglishPageRemoveProblemCommand request, CancellationToken cancellationToken)
        {
            var problem =
                await _context.EnglishPageProblem
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (problem == null)
                return CommandResponse.Failure(400, "The operation failed");

            _context.EnglishPageProblem.Remove(problem);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Problem  with id {request.Id} removed from english page by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
