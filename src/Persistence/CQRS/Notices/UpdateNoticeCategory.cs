using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class UpdateNoticeCategoryCommandHandler : IRequestHandler<UpdateNoticeCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateNoticeCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateNoticeCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.NoticeCategory.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (category == null)
                return CommandResponse.Failure(400, "ایتم مورد نظر در سیستم وجود ندارد");

            category.Order = request.Order;
            category.Title = request.Title;
            category.Description = request.Description;

            _context.NoticeCategory.Update(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
