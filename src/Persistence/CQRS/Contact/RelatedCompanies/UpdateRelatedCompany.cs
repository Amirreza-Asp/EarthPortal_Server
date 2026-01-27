using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.RelatedCompanies
{
    public class UpdateRelatedCompanyCommandHandler
        : IRequestHandler<UpdateRelatedCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UpdateRelatedCompanyCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateRelatedCompanyCommandHandler(
            ApplicationDbContext context,
            IPhotoManager photoManager,
            IWebHostEnvironment env,
            ILogger<UpdateRelatedCompanyCommandHandler> logger,
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
            UpdateRelatedCompanyCommand request,
            CancellationToken cancellationToken
        )
        {
            var upload = _env.WebRootPath + SD.RelatedCompanyPath;
            var relatedCompany = await _context.RelatedCompany.FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken
            );

            if (relatedCompany == null)
                return CommandResponse.Failure(400, "سازمان همکار در سیستم وجود ندارد");

            relatedCompany.Name = request.Name;
            relatedCompany.Order = request.Order;
            relatedCompany.Link = request.Link;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            String imgName = relatedCompany.Image;
            var oldImage = relatedCompany.Image;

            if (request.Image != null)
            {
                imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                _photoManager.Save(request.Image, upload + imgName);

                relatedCompany.Image = imgName;
            }

            _context.RelatedCompany.Update(relatedCompany);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Image != null && File.Exists(upload + oldImage))
                    File.Delete(upload + oldImage);

                _logger.LogInformation(
                    "RelatedCompany with id {Username} updated by {UserRealName} in {DoneTime}",
                    relatedCompany.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(imgName);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
