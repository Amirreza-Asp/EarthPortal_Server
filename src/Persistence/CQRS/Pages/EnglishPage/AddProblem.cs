using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageAddProblemCommandHandler : IRequestHandler<EnglishPageAddProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnglishPageAddProblemCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public EnglishPageAddProblemCommandHandler(ApplicationDbContext context, ILogger<EnglishPageAddProblemCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(EnglishPageAddProblemCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.Content))
                return CommandResponse.Failure(400, "Enter the content of the Problem");

            var englishPageId =
                await _context.EnglishPage
                    .Select(b => b.Id)
                    .FirstAsync(cancellationToken);

            var problem = new EnglishProblem(request.Content, englishPageId);

            _context.EnglishPageProblem.Add(problem);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Problem with content  {request.Content} added to english page by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(problem.Id);
            }

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
