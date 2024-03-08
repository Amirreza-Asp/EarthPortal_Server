using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.RelatedCompanies
{
    public class UpdateRelatedCompanyCommandHandler : IRequestHandler<UpdateRelatedCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoManager _photoManager;
        private readonly IHostingEnvironment _env;

        public UpdateRelatedCompanyCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(UpdateRelatedCompanyCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath + SD.RelatedCompanyPath;
            var relatedCompany = await _context.RelatedCompany.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (relatedCompany == null)
                return CommandResponse.Failure(400, "سازمان همکار در سیستم وجود ندارد");

            relatedCompany.Name = request.Name;
            relatedCompany.Order = request.Order;


            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            String imgName = String.Empty;

            if (request.Image != null)
            {
                imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                if (File.Exists(upload + relatedCompany.Image))
                    File.Delete(upload + relatedCompany.Image);

                await _photoManager.SaveAsync(request.Image, upload + imgName, cancellationToken);

                relatedCompany.Image = imgName;
            }

            _context.RelatedCompany.Update(relatedCompany);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(imgName);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }

    }
}
