using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.About;
using Application.Models;
using Domain;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.About
{
    public class CreateAboutCommandHandler : IRequestHandler<CreateAboutCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateAboutCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateAboutCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env, IUserAccessor userAccessor, ILogger<CreateAboutCommandHandler> logger)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(CreateAboutCommand request, CancellationToken cancellationToken)
        {
            try
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

                    _photoManager.Save(request.Image, upload + imgName);

                    var about = new AboutUs(request.Title, request.Description, null, imgName, request.AccomplishedDate);
                    about.Order = request.Order;
                    _context.AboutUs.Add(about);


                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        _logger.LogInformation($"about with id {about.Id} registered by {_userAccessor.GetUserName()} in {DateTime.Now}");
                        return CommandResponse.Success(new { id = about.Id, image = imgName });
                    }
                }
                else
                {
                    var about = new AboutUs(request.Title, request.Description, request.Video, null, request.AccomplishedDate);
                    about.Order = request.Order;
                    _context.AboutUs.Add(about);


                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        _logger.LogInformation($"about with id {about.Id} registered by {_userAccessor.GetUserName()} in {DateTime.Now}");
                        return CommandResponse.Success(new { id = about.Id });
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
