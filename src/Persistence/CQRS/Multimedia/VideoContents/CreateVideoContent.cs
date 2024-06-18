using Application.CQRS.Multimedia.VideoContents;
using Application.Models;
using Domain.Entities.Mutimedia;
using MediatR;

namespace Persistence.CQRS.Multimedia.VideoContents
{
    public class CreateVideoContentCommandHandler : IRequestHandler<CreateVideoContentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateVideoContentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateVideoContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var video = new VideoContent(request.Title, request.Description, request.Video, request.Link);
            video.Order = request.Order;
            video.CreatedAt = request.CreatedAt;
            _context.Add(video);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(video.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
