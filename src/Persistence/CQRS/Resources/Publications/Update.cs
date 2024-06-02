using Application.CQRS.Resources.Publications;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Publications
{
    public class UpdatePublicationCommandHandler : IRequestHandler<UpdatePublicationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdatePublicationCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdatePublicationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Publication.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "انتشارات مورد نظر در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Title = request.Title;

            _context.Publication.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
