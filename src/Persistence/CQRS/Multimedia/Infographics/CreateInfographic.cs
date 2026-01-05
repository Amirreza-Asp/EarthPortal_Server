using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using Domain;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Multimedia.Infographics
{
    public class CreateInfographicCommandHandler
        : IRequestHandler<CreateInfographicCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CreateInfographicCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateInfographicCommandHandler(
            ApplicationDbContext context,
            IPhotoManager photoManager,
            IWebHostEnvironment env,
            ILogger<CreateInfographicCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateInfographicCommand request,
            CancellationToken cancellationToken
        )
        {
            var upload = _env.WebRootPath + SD.InfographicPath;
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            _photoManager.Save(request.Image, upload + imgName);

            var infographic = new Infographic(imgName, request.Title);
            infographic.Order = request.Order;
            infographic.IsLandscape = request.IsLandscape;
            infographic.CreatedAt = request.CreatedAt;

            _context.Infographic.Add(infographic);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    $"Infographic with id {infographic.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success(new { Name = imgName, Id = infographic.Id });
            }
            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
