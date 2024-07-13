using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class CreateGalleryCommandHandler : IRequestHandler<CreateGalleryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateGalleryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateGalleryCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, ILogger<CreateGalleryCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateGalleryCommand request, CancellationToken cancellationToken)
        {
            var gallery = new Domain.Entities.Mutimedia.Gallery(request.Title, request.Description);
            gallery.Order = request.Order;
            gallery.CreatedAt = request.CreatedAt;
            _context.Gallery.Add(gallery);

            var images = new List<GalleryPhoto>();

            var upload = _env.WebRootPath + SD.GalleryPath;
            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            foreach (var img in request.Images)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                _photoManager.Save(img, upload + imgName);

                var galleryImage = new GalleryPhoto(imgName, 0, gallery.Id);
                images.Add(galleryImage);

                _context.GalleryPhoto.Add(galleryImage);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Gallery with id {gallery.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = gallery.Id, Images = images.Select(s => new { Id = s.Id, Name = s.Name }) });
            }
            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
