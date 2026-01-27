using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Infos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Infos
{
    public class RemoveGeoAddressCommandHandler
        : IRequestHandler<RemoveGeoAddressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveGeoAddressCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveGeoAddressCommandHandler(
            ApplicationDbContext context,
            ILogger<RemoveGeoAddressCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveGeoAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var geoAddress = await _context.GeoAddress.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (geoAddress == null)
                return CommandResponse.Failure(400, "آدرس انتخاب شده در سیستم وجود ندارد");

            _context.GeoAddress.Remove(geoAddress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "GeoAddress with id {Username} removed by {UserRealName} in {DoneTime}",
                    geoAddress.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با  شکست مواجه شد");
        }
    }
}
