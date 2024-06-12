using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.About
{
    public class UpdateAboutCommandHandler : IRequestHandler<UpdateAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IHostingEnvironment _env;

        public UpdateAboutCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(UpdateAboutCommand request, CancellationToken cancellationToken)
        {
            if (request.IsVideo && String.IsNullOrEmpty(request.Video))
                return CommandResponse.Failure(400, "ویدیو را وارد کنید");

            if (request.IsVideo && !request.Video.Contains("iframe"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نمی باشد");



            var about = await _context.AboutUs.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (about == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            about.Title = request.Title;
            about.Content = request.Description;
            about.CreatedAt = request.CreatedAt;
            about.Order = request.Order;
            var upload = _env.WebRootPath + SD.AboutUsPath;

            if (!request.IsVideo)
            {
                var oldImage = about.Image;
                if (request.Image != null)
                    about.Image = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

                about.Video = null;

                _context.AboutUs.Update(about);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {

                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    if (request.Image != null)
                    {
                        if (oldImage != null && File.Exists(upload + oldImage))
                            File.Delete(upload + oldImage);

                        await _photoManager.SaveAsync(request.Image, upload + about.Image, cancellationToken);
                    }

                    return CommandResponse.Success(new { Image = about.Image });
                }
            }
            else
            {
                var oldImage = about.Image;
                about.Video = request.Video;
                about.Image = null;

                _context.AboutUs.Update(about);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {

                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    if (oldImage != null && File.Exists(upload + oldImage))
                        File.Delete(upload + oldImage);

                    return CommandResponse.Success();
                }
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
