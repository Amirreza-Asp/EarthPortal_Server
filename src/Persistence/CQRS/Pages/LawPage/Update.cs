using Application.CQRS.Pages.LawPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.LawPage
{
    public class UpdateLawPageCommandHandler : IRequestHandler<UpdateLawPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateLawPageCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateLawPageCommand request, CancellationToken cancellationToken)
        {
            var entity =
                await _context.LawPage
                    .FirstAsync(cancellationToken);

            entity.WarningTitle = request.WarningTitle;
            entity.WarningContent = request.WarningContent;

            _context.LawPage.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
