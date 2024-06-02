using Application.CQRS.Regulation.ApprovalAuthorities;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;

namespace Persistence.CQRS.Regulation.ApprovalAuthorities
{
    public class CreateApprovalAuthorityCommandHandler : IRequestHandler<CreateApprovalAuthorityCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateApprovalAuthorityCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateApprovalAuthorityCommand request, CancellationToken cancellationToken)
        {
            var entity = new ApprovalAuthority(request.Title);
            entity.Order = request.Order;
            _context.ApprovalAuthority.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
