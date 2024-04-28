using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Pages.EnglishPage
{
    public class UpdateCurrentSituationImageCommandHandler : IRequestHandler<UpdateCurrentSituationImageCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public UpdateCurrentSituationImageCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
        }

        public async Task<CommandResponse> Handle(UpdateCurrentSituationImageCommand request, CancellationToken cancellationToken)
        {
            var page =
                await _context.EnglishPage
                    .FirstAsync(cancellationToken);

            var newImageName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
            var oldImageName = page.CurrentSituation.Image;
            var upload = _env.WebRootPath + SD.EnglishPageCurrentSituationImage;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            page.CurrentSituation.Image = newImageName;

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (File.Exists(upload + oldImageName))
                    File.Delete(upload + oldImageName);

                await _photoManager.SaveAsync(request.Image, upload + newImageName, cancellationToken);

                return CommandResponse.Success(newImageName);
            }

            return CommandResponse.Failure(400, "Photo editing failed");
        }
    }
}
