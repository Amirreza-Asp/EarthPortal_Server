using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Multimedia.Infographics
{
    public class RemoveInfographicCommandHandler : IRequestHandler<RemoveInfographicCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public RemoveInfographicCommandHandler(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<CommandResponse> Handle(RemoveInfographicCommand request, CancellationToken cancellationToken)
        {
            var infographic = await _context.Infographic.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (infographic == null)
                return CommandResponse.Failure(400, "اینفوگرافیک انتخاب شده در سیستم وجود ندارد");

            _context.Infographic.Remove(infographic);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
