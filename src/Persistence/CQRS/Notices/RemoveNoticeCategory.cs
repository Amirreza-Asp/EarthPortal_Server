using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class RemoveNoticeCategoryCommandHandler : IRequestHandler<RemoveNoticeCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveNoticeCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveNoticeCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.NoticeCategory.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (category == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            _context.Remove(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
