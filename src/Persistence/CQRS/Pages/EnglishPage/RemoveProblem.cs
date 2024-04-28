using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageRemoveProblemCommandHandler : IRequestHandler<EnglishPageRemoveProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageRemoveProblemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
