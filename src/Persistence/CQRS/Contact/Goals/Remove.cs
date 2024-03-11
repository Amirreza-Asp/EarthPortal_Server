using Application.CQRS.Contact.Goals;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.Goals
{
    public class RemoveGoalCommandHandler : IRequestHandler<RemoveGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveGoalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveGoalCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Goal.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "هدف مورد نظر در سیستم وجود ندارد");

            _context.Goal.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
