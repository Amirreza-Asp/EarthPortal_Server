using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Infographics
{
    public class RemoveInfographicCommandHandler
        : IRequestHandler<RemoveInfographicCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RemoveInfographicCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveInfographicCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<RemoveInfographicCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveInfographicCommand request,
            CancellationToken cancellationToken
        )
        {
            var infographic = await _context.Infographic.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (infographic == null)
                return CommandResponse.Failure(400, "اینفوگرافیک انتخاب شده در سیستم وجود ندارد");

            _context.Infographic.Remove(infographic);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Infographic with id {Username} removed by {UserRealName} in {DoneTime}",
                    infographic.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
