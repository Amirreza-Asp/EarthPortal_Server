using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateEnglishPageCommandHandler : IRequestHandler<UpdateEnglishPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateEnglishPageCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateEnglishPageCommand request, CancellationToken cancellationToken)
        {
            var englishPage =
                await _context.EnglishPage
                    .FirstAsync(cancellationToken);

            englishPage.Intro = request.Intro;
            englishPage.CurrentSituation = request.CurrentSituation;
            englishPage.MainIdea = request.MainIdea;
            englishPage.Vision = request.Vision;

            _context.EnglishPage.Update(englishPage);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "The operation failed");
        }
    }
}
