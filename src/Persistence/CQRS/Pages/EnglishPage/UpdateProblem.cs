using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageUpdateProblemCommandHandler : IRequestHandler<EnglishPageUpdateProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnglishPageUpdateProblemCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public EnglishPageUpdateProblemCommandHandler(ApplicationDbContext context, ILogger<EnglishPageUpdateProblemCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(EnglishPageUpdateProblemCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.Content))
                return CommandResponse.Failure(400, "Enter the content of the Problem");

            var problem =
                await _context.EnglishPageProblem
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync();

            if (problem == null)
                return CommandResponse.Failure(400, "The operation failed");

            problem.Content = request.Content;

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Problem updated from EnglishPage  by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
