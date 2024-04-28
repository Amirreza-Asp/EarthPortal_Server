using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageAddSolutionCommandHandler : IRequestHandler<EnglishPageAddSolutionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageAddSolutionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(EnglishPageAddSolutionCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.Content))
                return CommandResponse.Failure(400, "Enter the content of the Solution");

            var englishPageId =
               await _context.EnglishPage
                .Select(b => b.Id)
                .FirstAsync(cancellationToken);

            var solution = new EnglishSolution(request.Content, englishPageId);

            _context.EnglishPageSolution.Add(solution);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(solution.Id);

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
