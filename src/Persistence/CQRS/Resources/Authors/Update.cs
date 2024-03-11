using Application.CQRS.Resources.Authors;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Authors
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateAuthorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await _context.Author.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (author == null)
                return CommandResponse.Failure(400, "نویسنده مورد نظر در سیستم وجود ندارد");

            author.FullName = request.Name;

            _context.Author.Update(author);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
