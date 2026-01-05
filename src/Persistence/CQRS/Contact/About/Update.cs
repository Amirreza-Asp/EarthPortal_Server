using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.About
{
    public class UpdateAboutCommandHandler : IRequestHandler<UpdateAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UpdateAboutCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateAboutCommandHandler(
            ApplicationDbContext context,
            IPhotoManager photoManager,
            IWebHostEnvironment env,
            ILogger<UpdateAboutCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateAboutCommand request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                if (request.IsVideo && String.IsNullOrEmpty(request.Video))
                    return CommandResponse.Failure(400, "ویدیو را وارد کنید");

                if (request.IsVideo && !request.Video.Contains("iframe"))
                    return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نمی باشد");

                var about = await _context.AboutUs.FirstOrDefaultAsync(
                    b => b.Id == request.Id,
                    cancellationToken
                );

                if (about == null)
                    return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

                about.Title = request.Title;
                about.Content = request.Description;
                about.Order = request.Order;
                about.AccomplishedDate = request.AccomplishedDate;
                var upload = _env.WebRootPath + SD.AboutUsPath;

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                if (!request.IsVideo)
                {
                    var oldImage = about.Image;
                    if (request.Image != null)
                        about.Image = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

                    about.Video = null;
                    _context.AboutUs.Update(about);

                    if (request.Image != null)
                        _photoManager.Save(request.Image, upload + about.Image);

                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        if (
                            request.Image != null
                            && oldImage != null
                            && File.Exists(upload + oldImage)
                        )
                            File.Delete(upload + oldImage);

                        _logger.LogInformation(
                            $"about with id {about.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}"
                        );

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
                        if (oldImage != null && File.Exists(upload + oldImage))
                            File.Delete(upload + oldImage);

                        _logger.LogInformation(
                            $"about with id {about.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}"
                        );

                        return CommandResponse.Success();
                    }
                }

                return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
            }
            catch (Exception ex)
            {
                return CommandResponse.Failure(500, ex);
            }
        }
    }
}
