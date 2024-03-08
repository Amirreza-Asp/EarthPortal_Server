using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.EducationalVideos
{
    public class RemoveEducationalVideoCommandHandler : IRequestHandler<RemoveEducationalVideoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveEducationalVideoCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveEducationalVideoCommand request, CancellationToken cancellationToken)
        {
            var edv = await _context.EducationalVideo.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (edv == null)
                return CommandResponse.Failure(400, "ویدیو مورد نظر در سیستم وجود ندارد");

            _context.EducationalVideo.Remove(edv);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
