using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageUpdateProblemCommandHandler : IRequestHandler<EnglishPageUpdateProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageUpdateProblemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
