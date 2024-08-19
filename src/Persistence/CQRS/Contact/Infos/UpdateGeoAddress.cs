using Application.CQRS.Contact.Infos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Infos
{
    public class UpdateGeoAddressCommandHandler : IRequestHandler<UpdateGeoAddressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateGeoAddressCommandHandler> _logger;

        public UpdateGeoAddressCommandHandler(ApplicationDbContext context, ILogger<UpdateGeoAddressCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(UpdateGeoAddressCommand request, CancellationToken cancellationToken)
        {
            var geoAddress = await _context.GeoAddress.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (geoAddress == null)
                return CommandResponse.Failure(400, "شناسه وارد شده اشتباه است");

            geoAddress.IFrame = request.IFrame;
            geoAddress.Order = request.Order;
            geoAddress.Route = request.Route;
            geoAddress.RouteTitle = request.RouteTitle;

            _context.GeoAddress.Update(geoAddress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
