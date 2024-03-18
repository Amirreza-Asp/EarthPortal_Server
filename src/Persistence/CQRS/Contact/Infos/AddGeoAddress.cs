using Application.CQRS.Contact.Infos;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Infos
{
    public class AddGeoAddressCommandHandler : IRequestHandler<AddGeoAddressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public AddGeoAddressCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(AddGeoAddressCommand request, CancellationToken cancellationToken)
        {
            var infoId = await _context.Info.Select(b => b.Id).FirstAsync(cancellationToken);

            var geoAddress = new GeoAddress(request.Lat, request.Lon, request.Route, infoId);
            _context.GeoAddress.Add(geoAddress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(geoAddress.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
