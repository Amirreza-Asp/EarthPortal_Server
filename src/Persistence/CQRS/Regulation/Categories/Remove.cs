using Application.CQRS.Regulation.Categories;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.Categories
{
    public class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.LawCategory.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " موضوع انتخاب شده در سیستم وجود ندارد");


            _context.LawCategory.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
