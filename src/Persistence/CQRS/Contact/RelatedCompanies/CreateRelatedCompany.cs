using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Domain;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.RelatedCompanies
{
    public class CreateRelatedCompanyCommandHandler
        : IRequestHandler<CreateRelatedCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CreateRelatedCompanyCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateRelatedCompanyCommandHandler(
            ApplicationDbContext context,
            IPhotoManager photoManager,
            IWebHostEnvironment env,
            ILogger<CreateRelatedCompanyCommandHandler> logger,
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
            CreateRelatedCompanyCommand request,
            CancellationToken cancellationToken
        )
        {
            var upload = _env.WebRootPath + SD.RelatedCompanyPath;
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            var relatedCompany = new RelatedCompany(request.Name, imgName, request.Order);
            relatedCompany.Link = request.Link;

            _context.RelatedCompany.Add(relatedCompany);

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            _photoManager.Save(request.Image, upload + imgName);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    $"RelatedCompany with id {relatedCompany.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success(
                    new { Id = relatedCompany.Id, Image = relatedCompany.Image }
                );
            }
            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
