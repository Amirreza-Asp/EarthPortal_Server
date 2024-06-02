using Application.CQRS.Contact.Goals;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;

namespace Persistence.CQRS.Contact.Goals
{
    public class CreateGoalCommandHandler : IRequestHandler<CreateGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateGoalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
        {
            var entity = new Goal(request.Title);
            entity.Order = request.Order;

            _context.Goal.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
