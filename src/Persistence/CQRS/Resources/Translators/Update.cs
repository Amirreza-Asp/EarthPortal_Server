using Application.CQRS.Resources.Translators;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Translators
{
    public class UpdateTranslatorCommandHandler : IRequestHandler<UpdateTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTranslatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateTranslatorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Translator.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "مترجم مورد نظر در سیستم وجود ندارد");

            entity.FullName = request.Name;

            _context.Translator.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
