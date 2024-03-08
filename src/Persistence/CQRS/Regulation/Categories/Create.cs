using Application.CQRS.Regulation.Categories;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;

namespace Persistence.CQRS.Regulation.Categories
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new LawCategory(request.Title);

            _context.LawCategory.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
