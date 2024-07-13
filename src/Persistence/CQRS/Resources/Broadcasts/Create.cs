using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Brodcasts;
using Application.Models;
using Domain;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Broadcasts
{
    public class CreateBroadcastCommandHandler : IRequestHandler<CreateBroadcastCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateBroadcastCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateBroadcastCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env, ILogger<CreateBroadcastCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateBroadcastCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.BroadcastImagePath))
                Directory.CreateDirectory(upload + SD.BroadcastImagePath);

            if (!Directory.Exists(upload + SD.BroadcastFilePath))
                Directory.CreateDirectory(upload + SD.BroadcastFilePath);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);


            var entity = new Broadcast(request.Title, request.Description, fileName, request.PublishDate, request.AuthorId, imgName, request.ShortDescription, request.Pages, request.TranslatorId, request.PublicationId);
            entity.Order = request.Order;
            _context.Broadcast.Add(entity);

            using (Stream fileStream = new FileStream(upload + SD.BroadcastFilePath + fileName, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            _photoManager.Save(request.Image, upload + SD.BroadcastImagePath + imgName);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Broadcast with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = entity.Id, Image = entity.Image, File = entity.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
