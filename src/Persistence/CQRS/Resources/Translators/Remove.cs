using Application.CQRS.Resources.Translators;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Translators
{
    public class RemoveTranslatorCommandHandler : IRequestHandler<RemoveTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveTranslatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveTranslatorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Translator.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "مترجم مورد نظر در سیستم وجود ندارد");

            _context.Translator.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
