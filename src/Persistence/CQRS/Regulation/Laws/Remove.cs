using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Laws
{
    public class RemoveLawCommandHandler : IRequestHandler<RemoveLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<RemoveLawCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveLawCommandHandler(ApplicationDbContext context, IHostingEnvironment env, ILogger<RemoveLawCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveLawCommand request, CancellationToken cancellationToken)
        {
            var law = await _context.Law.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (law == null)
                return CommandResponse.Failure(400, "قانون انتخاب شده در سیستم وجود ندارد");

            _context.Law.Remove(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (File.Exists(_env.WebRootPath + SD.LawPdfPath + law.Pdf))
                    File.Delete(_env.WebRootPath + SD.LawPdfPath + law.Pdf);

                _logger.LogInformation($"Law with id {law.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
