using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Infographics
{
    public class UpdateInfographicCommandHandler
        : IRequestHandler<UpdateInfographicCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateInfographicCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateInfographicCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IPhotoManager photoManager,
            ILogger<UpdateInfographicCommandHandler> logger,
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
            UpdateInfographicCommand request,
            CancellationToken cancellationToken
        )
        {
            var infographic = await _context.Infographic.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (infographic == null)
                return CommandResponse.Failure(400, "اینفوگرافیک مورد نظر در سیستم وجود ندارد");

            infographic.Order = request.Order;
            infographic.Title = request.Title;
            infographic.IsLandscape = request.IsLandscape;
            infographic.CreatedAt = request.CreatedAt;

            var oldImageName = infographic.Name;

            if (request.Image != null)
                infographic.Name = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.InfographicPath))
                Directory.CreateDirectory(upload + SD.InfographicPath);

            if (request.Image != null)
                _photoManager.Save(request.Image, upload + SD.InfographicPath + infographic.Name);

            _context.Infographic.Update(infographic);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Image != null)
                {
                    if (File.Exists(upload + SD.InfographicPath + oldImageName))
                        File.Delete(upload + SD.InfographicPath + oldImageName);
                }

                _logger.LogInformation(
                    "Infographic with id {Username} updated by {UserRealName} in {DoneTime}",
                    infographic.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(infographic.Name);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
