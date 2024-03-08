using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Domain;
using Domain.Dtos.Contact;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Contact.RelatedCompanies
{
    public class CreateRelatedCompanyCommandHandler : IRequestHandler<CreateRelatedCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IHostingEnvironment _env;

        public CreateRelatedCompanyCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(CreateRelatedCompanyCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath + SD.RelatedCompanyPath;
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            var relatedCompany = new RelatedCompany(request.Name, imgName, request.Order);

            _context.RelatedCompany.Add(relatedCompany);

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            await _photoManager.SaveAsync(request.Image, upload + imgName, cancellationToken);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { Id = relatedCompany.Id, Image = relatedCompany.Image });

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
