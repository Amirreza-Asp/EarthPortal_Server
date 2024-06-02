using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.EducationalVideos
{
    public class UpdateEducationalVideoCommandHandler : IRequestHandler<UpdateEducationalVideoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateEducationalVideoCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateEducationalVideoCommand request, CancellationToken cancellationToken)
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var edv = await _context.EducationalVideo.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (edv == null)
                return CommandResponse.Failure(400, "ویدیو مورد نظر در سیستم وجود ندارد");

            edv.Title = request.Title;
            edv.Description = request.Description;
            edv.Video = request.Video;
            edv.Order = request.Order;

            _context.EducationalVideo.Update(edv);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
