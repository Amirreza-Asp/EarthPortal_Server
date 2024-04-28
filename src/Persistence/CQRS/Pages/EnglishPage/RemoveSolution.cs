using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageRemoveSolutionCommandHandler : IRequestHandler<EnglishPageRemoveSolutionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageRemoveSolutionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(EnglishPageRemoveSolutionCommand request, CancellationToken cancellationToken)
        {
            var solution =
                await _context.EnglishPageSolution
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (solution == null)
                return CommandResponse.Failure(400, "The operation failed");

            _context.EnglishPageSolution.Remove(solution);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
