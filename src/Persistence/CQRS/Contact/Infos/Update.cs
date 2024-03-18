using Application.CQRS.Contact.CommonicationWays;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Infos
{
    public class UpdateInfoCommandHandler : IRequestHandler<UpdateInfoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateInfoCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateInfoCommand request, CancellationToken cancellationToken)
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

            _context.Info.Update(info);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
