using Application.CQRS.Notices;
using Application.Models;
using Domain.Entities.Notices;
using MediatR;

namespace Persistence.CQRS.Notices
{
    public class CreateNoticeCategoryCommandHandler : IRequestHandler<CreateNoticeCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateNoticeCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateNoticeCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new NoticeCategory(request.Title, request.Description);
            category.Order = request.Order;
            _context.NoticeCategory.Add(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(category.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
