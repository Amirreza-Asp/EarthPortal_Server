using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;

namespace Persistence.CQRS.Contact.EducationalVideos
{
    public class CreateEducationalVideoCommandHandler : IRequestHandler<CreateEducationalVideoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateEducationalVideoCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateEducationalVideoCommand request, CancellationToken cancellationToken)
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var edv = new EducationalVideo(request.Title, request.Description, request.Video);
            edv.Order = request.Order;

            _context.EducationalVideo.Add(edv);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(edv.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
