using Application.CQRS.Resources.Authors;
using Application.Models;
using Domain.Entities.Resources;
using MediatR;

namespace Persistence.CQRS.Resources.Authors
{
    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateAuthorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = new Author(request.Name);
            author.Order = request.Order;
            _context.Author.Add(author);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(author.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
