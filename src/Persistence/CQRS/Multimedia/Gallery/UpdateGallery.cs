using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class UpdateGalleryCommandHandler : IRequestHandler<UpdateGalleryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public UpdateGalleryCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
        }

        public async Task<CommandResponse> Handle(UpdateGalleryCommand request, CancellationToken cancellationToken)
        {
            var gallery =
                await _context.Gallery.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (gallery == null)
                return CommandResponse.Failure(400, "آلبوم تصاویر انتخاب شده در سیستم وجود ندارد");

            gallery.Order = request.Order;
            gallery.Title = request.Title;
            gallery.Description = request.Description;
            gallery.CreatedAt = request.CreatedAt;

            _context.Gallery.Update(gallery);
            var upload = _env.WebRootPath + SD.GalleryPath;

            if (request.DeletedImages != null)
            {
                var deletedPhotos = await _context.GalleryPhoto.Where(b => request.DeletedImages.Contains(b.Id)).ToListAsync();

                foreach (var deletedPhoto in deletedPhotos)
                {
                    if (File.Exists(upload + deletedPhoto.Name))
                        File.Delete(upload + deletedPhoto.Name);

                    _context.GalleryPhoto.Remove(deletedPhoto);
                }
            }

            List<ImageSummary> images = new List<ImageSummary>();
            if (request.Images != null)
            {
                foreach (var img in request.Images)
                {
                    var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);

                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    await _photoManager.SaveAsync(img, upload + imgName, cancellationToken);

                    var galleryImage = new GalleryPhoto(imgName, 0, gallery.Id);
                    _context.GalleryPhoto.Add(galleryImage);

                    images.Add(new ImageSummary { Id = galleryImage.Id, Name = galleryImage.Name, Order = galleryImage.Order });
                }
            }


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(images);


            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");

        }
    }
}
