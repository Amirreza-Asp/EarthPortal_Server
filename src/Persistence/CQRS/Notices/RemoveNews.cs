using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{

    public class RemoveNewsCommandHandler : IRequestHandler<RemoveNewsCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public RemoveNewsCommandHandler(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<CommandResponse> Handle(RemoveNewsCommand request, CancellationToken cancellationToken)
        {
            var news = await _context.News.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (news == null)
                return CommandResponse.Failure(400, "خبر مورد نظر در سیستم وجود ندارد");

            _context.News.Remove(news);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "حذف با شکست مواجه شد");
        }
    }
}
