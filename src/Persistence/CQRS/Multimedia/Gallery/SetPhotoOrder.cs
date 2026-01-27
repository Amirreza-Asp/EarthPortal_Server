using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Gallery
{
    public class SetPhotoOrderCommandResponse
        : IRequestHandler<SetPhotoOrderCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SetPhotoOrderCommandResponse> _logger;
        private readonly IUserAccessor _userAccessor;

        public SetPhotoOrderCommandResponse(
            ApplicationDbContext context,
            ILogger<SetPhotoOrderCommandResponse> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            SetPhotoOrderCommand request,
            CancellationToken cancellationToken
        )
        {
            var galleryPhotos = await _context
                .GalleryPhoto.Where(b => b.GalleryId == request.Id)
                .ToListAsync(cancellationToken);

            if (galleryPhotos == null || !galleryPhotos.Any())
                return CommandResponse.Failure(400, "گالری انتخاب شده در سیستم وجود ندارد");

            foreach (var img in galleryPhotos)
                img.Order = request.Images.Find(b => b.Id == img.Id).Order;

            _context.GalleryPhoto.UpdateRange(galleryPhotos);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "Photo for Gallery with id {Username} sorted by {UserRealName} in {DoneTime}",
                    galleryPhotos.First().GalleryId,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
