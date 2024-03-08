using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Multimedia.VideoContents
{
    public class RemoveVideoContentCommandHandler : IRequestHandler<RemoveVideoContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveVideoContentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveVideoContentCommand request, CancellationToken cancellationToken)
        {
            var video = await _context.VideoContent.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (video == null)
                return CommandResponse.Failure(400, "ویدیو در سیستم وجود ندارد");

            _context.VideoContent.Remove(video);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
