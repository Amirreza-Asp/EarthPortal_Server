using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.CommonicationWays;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.Infos
{
    public class UpdateInfoCommandHandler : IRequestHandler<UpdateInfoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateInfoCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateInfoCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateInfoCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateInfoCommand request,
            CancellationToken cancellationToken
        )
        {
            var info = await _context.Info.FirstOrDefaultAsync(cancellationToken);

            if (info == null)
                return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");

            info.PhoneNumber = request.PhoneNumber;
            info.BaleLink = request.BaleLink;
            info.AparatLink = request.AparatLink;
            info.RubikaLink = request.RubikaLink;
            info.Email = request.Email;
            info.EitaaLink = request.EitaaLink;
            info.GapLink = request.GapLink;
            info.IGapLink = request.IGapLink;

            _context.Info.Update(info);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Info with id {Username} updated by {UserRealName} in {DoneTime}",
                    info.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
