using Application.CQRS.Pages.AboutUsPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.AboutUsPage
{
    public class UpdateAboutUsPageCommandHandler : IRequestHandler<UpdateAboutUsPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateAboutUsPageCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateAboutUsPageCommand request, CancellationToken cancellationToken)
        {
            var entity =
                await _context.AboutUsPage
                      .FirstAsync(cancellationToken);

            entity.Footer = request.Footer;
            entity.Title = request.Title;
            entity.Content = request.Content;

            _context.AboutUsPage.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
