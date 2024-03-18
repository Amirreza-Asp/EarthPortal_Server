using Application.CQRS.Contact.Infos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Infos
{
    public class RemoveGeoAddressCommandHandler : IRequestHandler<RemoveGeoAddressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveGeoAddressCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveGeoAddressCommand request, CancellationToken cancellationToken)
        {
            var geoAddress = await _context.GeoAddress.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (geoAddress == null)
                return CommandResponse.Failure(400, "آدرس انتخاب شده در سیستم وجود ندارد");

            _context.GeoAddress.Remove(geoAddress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با  شکست مواجه شد");
        }
    }
}
