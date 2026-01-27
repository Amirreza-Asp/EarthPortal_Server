using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class RemoveGalleryCommandHandler
        : IRequestHandler<RemoveGalleryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<RemoveGalleryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveGalleryCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IPhotoManager photoManager,
            ILogger<RemoveGalleryCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            RemoveGalleryCommand request,
            CancellationToken cancellationToken
        )
        {
            var gallery = await _context
                .Gallery.Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (gallery == null)
                return CommandResponse.Failure(400, "آلبوم تصاویر انتخاب شده در سیستم وجود ندارد");

            var upload = _env.WebRootPath + SD.GalleryPath;

            _context.GalleryPhoto.RemoveRange(gallery.Images);
            _context.Gallery.Remove(gallery);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                foreach (var deletedPhoto in gallery.Images)
                {
                    if (File.Exists(upload + deletedPhoto.Name))
                        File.Delete(upload + deletedPhoto.Name);
                }

                _logger.LogInformation(
                    "Gallery with id {Username} removed by {UserRealName} in {DoneTime}",
                    gallery.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
