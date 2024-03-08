using Application.CQRS.Notices;
using Application.Models;
using Domain.Entities.Notices;
using MediatR;

namespace Persistence.CQRS.Notices
{
    public class CreateNewsCategoryCommandHandler : IRequestHandler<CreateNewsCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateNewsCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new NewsCategory(request.Title, request.Description);

            _context.NewsCategory.Add(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(category.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
