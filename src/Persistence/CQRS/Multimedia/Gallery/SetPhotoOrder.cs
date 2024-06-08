using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class SetPhotoOrderCommandResponse : IRequestHandler<SetPhotoOrderCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public SetPhotoOrderCommandResponse(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(SetPhotoOrderCommand request, CancellationToken cancellationToken)
        {
            var galleryPhotos =
                await _context.GalleryPhoto
                    .Where(b => b.GalleryId == request.Id)
                    .ToListAsync(cancellationToken);

            if (galleryPhotos == null || !galleryPhotos.Any())
                return CommandResponse.Failure(400, "گالری انتخاب شده در سیستم وجود ندارد");


            foreach (var img in galleryPhotos)
                img.Order = request.Images.Find(b => b.Id == img.Id).Order;

            _context.GalleryPhoto.UpdateRange(galleryPhotos);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
