using Application.CQRS.Regulation.ApprovalTypes;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ApprovalTypes
{
    public class RemoveApprovalTypeCommandHandler : IRequestHandler<RemoveApprovalTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveApprovalTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveApprovalTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApprovalType.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " نوع مصوبه انتخاب شده در سیستم وجود ندارد");


            _context.ApprovalType.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
