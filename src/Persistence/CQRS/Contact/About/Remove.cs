using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.About
{
    public class RemoveAboutCommandHandler : IRequestHandler<RemoveAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<RemoveAboutCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveAboutCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IUserAccessor userAccessor, ILogger<RemoveAboutCommandHandler> logger)
        {
            _context = context;
            _env = env;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(RemoveAboutCommand request, CancellationToken cancellationToken)
        {
            var about = await _context.AboutUs.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (about == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            var upload = _env.WebRootPath + SD.AboutUsPath;

            _context.AboutUs.Remove(about);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                if (File.Exists(upload + about.Image))
                    File.Delete(upload + about.Image);

                _logger.LogInformation($"about with id {about.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
