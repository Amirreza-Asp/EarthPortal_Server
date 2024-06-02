using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class CreateApprovalStatusCommandHandler : IRequestHandler<CreateApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateApprovalStatusCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateApprovalStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.Regulation.ApprovalStatus(request.Title);
            entity.Order = request.Order;
            _context.ApprovalStatus.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
