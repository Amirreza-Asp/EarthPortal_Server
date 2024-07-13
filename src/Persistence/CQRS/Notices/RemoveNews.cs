using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Notices
{

    public class RemoveNewsCommandHandler : IRequestHandler<RemoveNewsCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<RemoveNewsCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveNewsCommandHandler(ApplicationDbContext context, IHostingEnvironment env, ILogger<RemoveNewsCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveNewsCommand request, CancellationToken cancellationToken)
        {
            var news = await _context.News.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (news == null)
                return CommandResponse.Failure(400, "خبر مورد نظر در سیستم وجود ندارد");

            _context.News.Remove(news);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"News with id {news.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "حذف با شکست مواجه شد");
        }
    }
}
