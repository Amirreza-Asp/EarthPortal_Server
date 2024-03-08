using Application.CQRS.Regulation.ApprovalAuthorities;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ApprovalAuthorities
{
    public class RemoveApprovalAuthorityCommandHandler : IRequestHandler<RemoveApprovalAuthorityCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveApprovalAuthorityCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveApprovalAuthorityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalAuthority.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "مرجع تصویب انتخاب شده در سیستم وجود ندارد");


            _context.ApprovalAuthority.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
