using Application.CQRS.Resources.Translators;
using Application.Models;
using Domain.Dtos.Resources;
using MediatR;

namespace Persistence.CQRS.Resources.Translators
{
    public class CreateTranslatorCommandHandler : IRequestHandler<CreateTranslatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateTranslatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateTranslatorCommand request, CancellationToken cancellationToken)
        {
            var entity = new Translator(request.Name);
            entity.Order = request.Order;
            _context.Translator.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
