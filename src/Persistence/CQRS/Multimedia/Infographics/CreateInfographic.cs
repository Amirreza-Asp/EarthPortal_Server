using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Multimedia.Infographics;
using Application.Models;
using Domain;
using Domain.Entities.Mutimedia;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Multimedia.Infographics
{
    public class CreateInfographicCommandHandler : IRequestHandler<CreateInfographicCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IHostingEnvironment _env;

        public CreateInfographicCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(CreateInfographicCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath + SD.InfographicPath;
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            await _photoManager.SaveAsync(request.Image, upload + imgName, cancellationToken);

            var infographic = new Infographic(imgName, request.Title);
            infographic.Order = request.Order;
            infographic.IsLandscape = request.IsLandscape;

            _context.Infographic.Add(infographic);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { Name = imgName, Id = infographic.Id });

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
