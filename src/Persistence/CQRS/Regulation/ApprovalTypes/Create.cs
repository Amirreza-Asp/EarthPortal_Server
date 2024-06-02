using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using MediatR;

namespace Persistence.CQRS.Regulation.ApprovalTypes
{
    public class CreateApprovalTypeCommandHandler : IRequestHandler<CreateApprovalTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateApprovalTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateApprovalTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.Regulation.ApprovalType(request.Title);
            entity.Order = request.Order;
            _context.ApprovalType.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
