using Application.CQRS.Contact.Goals;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Goals
{
    public class UpdateGoalCommandHandler : IRequestHandler<UpdateGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateGoalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Goal.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "هدف مورد نظر در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Title = request.Title;

            _context.Goal.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
