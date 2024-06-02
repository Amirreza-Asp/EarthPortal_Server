using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class CreateExecutorManagementCommandHandler : IRequestHandler<CreateExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateExecutorManagementCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = new ExecutorManagment(request.Title);
            entity.Order = request.Order;
            _context.ExecutorManagment.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
