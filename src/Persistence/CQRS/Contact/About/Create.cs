using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Contact.About
{
    public class CreateAboutCommandHandler : IRequestHandler<CreateAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public CreateAboutCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(CreateAboutCommand request, CancellationToken cancellationToken)
        {
            if (request.IsVideo && String.IsNullOrEmpty(request.Video))
                return CommandResponse.Failure(400, "ویدیو را وارد کنید");

            if (request.IsVideo && !request.Video.Contains("iframe"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نمی باشد");

            if (!request.IsVideo && request.Image == null)
                return CommandResponse.Failure(400, "عکس را وارد کنید");

            if (!request.IsVideo)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                var upload = _env.WebRootPath + SD.AboutUsPath;

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                await _photoManager.SaveAsync(request.Image, upload + imgName, cancellationToken);

                var about = new AboutUs(request.Title, request.Description, null, imgName);
                about.Order = request.Order;
                about.CreatedAt = request.CreatedAt;
                _context.AboutUs.Add(about);


                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success(new { id = about.Id, image = imgName });
            }
            else
            {
                var about = new AboutUs(request.Title, request.Description, request.Video, null);
                about.Order = request.Order;
                _context.AboutUs.Add(about);


                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success(new { id = about.Id });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
