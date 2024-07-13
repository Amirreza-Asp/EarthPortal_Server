using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.AboutUsPage;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.AboutUsPage
{
    public class UpdateAboutUsPageCommandHandler : IRequestHandler<UpdateAboutUsPageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateAboutUsPageCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateAboutUsPageCommandHandler(ApplicationDbContext context, ILogger<UpdateAboutUsPageCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
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
            {
                _logger.LogInformation($"AboutUsPage content updated by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
