using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class CreateGalleryCommandHandler : IRequestHandler<CreateGalleryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public CreateGalleryCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
        }

        public async Task<CommandResponse> Handle(CreateGalleryCommand request, CancellationToken cancellationToken)
        {
            var gallery = new Domain.Entities.Mutimedia.Gallery(request.Title, request.Description);
            gallery.Order = request.Order;
            _context.Gallery.Add(gallery);

            var images = new List<GalleryPhoto>();

            foreach (var img in request.Images)
            {
                var upload = _env.WebRootPath + SD.GalleryPath;
                var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                await _photoManager.SaveAsync(img, upload + imgName, cancellationToken);

                var galleryImage = new GalleryPhoto(imgName, 0, gallery.Id);
                images.Add(galleryImage);
                _context.GalleryPhoto.Add(galleryImage);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { Id = gallery.Id, Images = images.Select(s => new { Id = s.Id, Name = s.Name }) });

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
