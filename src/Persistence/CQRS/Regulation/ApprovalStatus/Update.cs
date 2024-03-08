using Application.CQRS.Regulation.ApprovalStatus;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ApprovalStatus
{
    public class UpdateApprovalStatusCommandHandler : IRequestHandler<UpdateApprovalStatusCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateApprovalStatusCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateApprovalStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalStatus.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " وضعیت مصوبه انتخاب شده در سیستم وجود ندارد");

            entity.Status = request.Title;

            _context.ApprovalStatus.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
