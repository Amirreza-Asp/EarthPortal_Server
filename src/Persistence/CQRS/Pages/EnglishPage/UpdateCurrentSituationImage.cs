using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateCurrentSituationImageCommandHandler
        : IRequestHandler<UpdateCurrentSituationImageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateCurrentSituationImageCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateCurrentSituationImageCommandHandler(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IPhotoManager photoManager,
            ILogger<UpdateCurrentSituationImageCommandHandler> logger,
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
            UpdateCurrentSituationImageCommand request,
            CancellationToken cancellationToken
        )
        {
            var page = await _context.EnglishPage.FirstAsync(cancellationToken);

            var newImageName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
            var oldImageName = page.CurrentSituation.Image;
            var upload = _env.WebRootPath + SD.EnglishPageCurrentSituationImage;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            page.CurrentSituation.Image = newImageName;

            _photoManager.Save(request.Image, upload + newImageName);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (File.Exists(upload + oldImageName))
                    File.Delete(upload + oldImageName);

                _logger.LogInformation(
                    $"CurrentSituationImage Updated from EnglishPage  by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );

                return CommandResponse.Success(newImageName);
            }

            return CommandResponse.Failure(400, "Photo editing failed");
        }
    }
}
