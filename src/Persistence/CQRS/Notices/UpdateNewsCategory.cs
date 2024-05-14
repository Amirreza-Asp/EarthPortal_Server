using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class UpdateNewsCategoryCommandHandler : IRequestHandler<UpdateNewsCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateNewsCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.NewsCategory.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (category == null)
                return CommandResponse.Failure(400, "ایتم مورد نظر در سیستم وجود ندارد");

            category.Title = request.Title;
            category.Description = request.Description;

            _context.NewsCategory.Update(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
