using Application.CQRS.Contact.Guids;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Guids
{
    public class UpdateGuideCommandHandler : IRequestHandler<UpdateGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateGuideCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = await _context.Guide.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (guide == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            guide.Title = request.Title;
            guide.IsPort = request.IsPort.ToLower() == "true";
            guide.Content = request.Content;
            guide.Order = request.Order;

            _context.Guide.Update(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(guide.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
