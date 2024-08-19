using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.Infos;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Infos
{
    public class AddGeoAddressCommandHandler : IRequestHandler<AddGeoAddressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddGeoAddressCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public AddGeoAddressCommandHandler(ApplicationDbContext context, ILogger<AddGeoAddressCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(AddGeoAddressCommand request, CancellationToken cancellationToken)
        {
            var infoId = await _context.Info.Select(b => b.Id).FirstAsync(cancellationToken);

            var geoAddress = new GeoAddress(request.IFrame, request.Route, infoId, request.RouteTitle, request.Order);
            _context.GeoAddress.Add(geoAddress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"GeoAddress with id {geoAddress.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(geoAddress.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
