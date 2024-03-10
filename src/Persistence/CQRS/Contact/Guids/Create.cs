using Application.CQRS.Contact.Guids;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;

namespace Persistence.CQRS.Contact.Guids
{
    public class CreateGuideCommandHandler : IRequestHandler<CreateGuideCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateGuideCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateGuideCommand request, CancellationToken cancellationToken)
        {
            var guide = new Guide(request.Title, request.Content, request.IsPort.ToLower() == "true");

            _context.Guide.Add(guide);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(guide.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
