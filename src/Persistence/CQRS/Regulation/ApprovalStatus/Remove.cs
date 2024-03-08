using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class RemoveApprovalStatusCommandHandler : IRequestHandler<RemoveApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveApprovalStatusCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveApprovalStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalStatus.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "وضعیت مصوبه انتخاب شده در سیستم وجود ندارد");


            _context.ApprovalStatus.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
