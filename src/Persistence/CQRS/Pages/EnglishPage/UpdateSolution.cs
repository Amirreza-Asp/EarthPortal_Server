using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageUpdateSolutionCommandHandler : IRequestHandler<EnglishPageUpdateSolutionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnglishPageUpdateSolutionCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public EnglishPageUpdateSolutionCommandHandler(ApplicationDbContext context, ILogger<EnglishPageUpdateSolutionCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(EnglishPageUpdateSolutionCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.Content))
                return CommandResponse.Failure(400, "Enter the content of the Solution");

            var solution =
               await _context.EnglishPageSolution
                   .Where(b => b.Id == request.Id)
                   .FirstOrDefaultAsync();

            if (solution == null)
                return CommandResponse.Failure(400, "The operation failed");

            solution.Content = request.Content;

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Solution updated from EnglishPage  by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
