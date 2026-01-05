using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Brodcasts;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Broadcasts
{
    public class UpdateBroadcastCommandHandler
        : IRequestHandler<UpdateBroadcastCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateBroadcastCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateBroadcastCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IPhotoManager photoManager,
            ILogger<UpdateBroadcastCommandHandler> logger,
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
            UpdateBroadcastCommand request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.Broadcast.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (entity == null)
                return CommandResponse.Failure(400, "نشریه مورد نظر در سیستم وجود ندارد");

            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.BroadcastImagePath))
                Directory.CreateDirectory(upload + SD.BroadcastImagePath);

            if (!Directory.Exists(upload + SD.BroadcastFilePath))
                Directory.CreateDirectory(upload + SD.BroadcastFilePath);

            var oldImage = entity.Image;
            var oldFile = entity.File;

            entity.Order = request.Order;
            entity.AuthorId = request.AuthorId;
            entity.ShortDescription = request.ShortDescription;
            entity.Description = request.Description;
            entity.Title = request.Title;

            entity.PublicationId = request.PublicationId;
            entity.Pages = request.Pages;
            entity.PublishDate = request.PublishDate;
            entity.TranslatorId = request.TranslatorId;

            if (request.File != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
                entity.File = fileName;

                using (
                    Stream fileStream = new FileStream(
                        upload + SD.BroadcastFilePath + entity.File,
                        FileMode.Create
                    )
                )
                {
                    await request.File.CopyToAsync(fileStream);
                }
            }

            if (request.Image != null)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                entity.Image = imgName;

                _photoManager.Save(request.Image, upload + SD.BroadcastImagePath + entity.Image);
            }

            _context.Broadcast.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Image != null)
                {
                    if (File.Exists(upload + SD.BroadcastImagePath + oldImage))
                        File.Delete(upload + SD.BroadcastImagePath + oldImage);
                }

                if (request.File != null)
                {
                    if (File.Exists(upload + SD.BroadcastFilePath + oldFile))
                        File.Delete(upload + SD.BroadcastFilePath + oldFile);
                }

                _logger.LogInformation(
                    $"Broadcast with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success(
                    new
                    {
                        Id = entity.Id,
                        Image = entity.Image,
                        File = entity.File
                    }
                );
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
