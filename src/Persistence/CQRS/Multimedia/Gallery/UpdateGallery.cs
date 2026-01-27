using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class UpdateGalleryCommandHandler
        : IRequestHandler<UpdateGalleryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateGalleryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateGalleryCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IPhotoManager photoManager,
            ILogger<UpdateGalleryCommandHandler> logger,
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
            UpdateGalleryCommand request,
            CancellationToken cancellationToken
        )
        {
            var upload = _env.WebRootPath + SD.GalleryPath;
            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            var gallery = await _context.Gallery.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (gallery == null)
                return CommandResponse.Failure(400, "آلبوم تصاویر انتخاب شده در سیستم وجود ندارد");

            gallery.Order = request.Order;
            gallery.Title = request.Title;
            gallery.Description = request.Description;
            gallery.CreatedAt = request.CreatedAt;

            _context.Gallery.Update(gallery);

            List<ImageSummary> images = new List<ImageSummary>();

            if (request.Images != null)
            {
                foreach (var img in request.Images)
                {
                    var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);

                    _photoManager.Save(img, upload + imgName);

                    var galleryImage = new GalleryPhoto(imgName, 0, gallery.Id);
                    _context.GalleryPhoto.Add(galleryImage);

                    images.Add(
                        new ImageSummary
                        {
                            Id = galleryImage.Id,
                            Name = galleryImage.Name,
                            Order = galleryImage.Order
                        }
                    );
                }
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.DeletedImages != null)
                {
                    var deletedPhotos = await _context
                        .GalleryPhoto.Where(b => request.DeletedImages.Contains(b.Id))
                        .ToListAsync();

                    foreach (var deletedPhoto in deletedPhotos)
                    {
                        if (File.Exists(upload + deletedPhoto.Name))
                            File.Delete(upload + deletedPhoto.Name);

                        _context.GalleryPhoto.Remove(deletedPhoto);
                    }
                }

                _logger.LogInformation(
                    "Gallery with id {Username} updated by {UserRealName} in {DoneTime}",
                    gallery.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(images);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
