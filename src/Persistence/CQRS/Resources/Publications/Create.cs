using Application.CQRS.Resources.Publications;
using Application.Models;
using Domain.Entities.Resources;
using MediatR;

namespace Persistence.CQRS.Resources.Publications
{
    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreatePublicationCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            var entity = new Publication(request.Title);

            _context.Publication.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(entity.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
