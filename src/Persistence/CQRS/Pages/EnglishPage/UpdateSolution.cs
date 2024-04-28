using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageUpdateSolutionCommandHandler : IRequestHandler<EnglishPageUpdateSolutionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageUpdateSolutionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
