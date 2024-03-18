using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.About
{
    public class RemoveAboutCommandHandler : IRequestHandler<RemoveAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public RemoveAboutCommandHandler(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
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

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
