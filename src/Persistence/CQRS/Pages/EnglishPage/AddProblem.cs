using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class EnglishPageAddProblemCommandHandler : IRequestHandler<EnglishPageAddProblemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public EnglishPageAddProblemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
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
                return CommandResponse.Success(problem.Id);

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
