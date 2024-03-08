using Application.CQRS.Contact.RelatedCompanies;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.RelatedCompanies
{
    public class RemoveRelatedCompanyCommandHandler : IRequestHandler<RemoveRelatedCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public RemoveRelatedCompanyCommandHandler(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<CommandResponse> Handle(RemoveRelatedCompanyCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath + SD.RelatedCompanyPath;
            var relatedCompany = await _context.RelatedCompany.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (relatedCompany == null)
                return CommandResponse.Failure(400, "سازمان همکار در سیستم وجود ندارد");

            if (File.Exists(upload + relatedCompany.Image))
                File.Delete(upload + relatedCompany.Image);

            _context.RelatedCompany.Remove(relatedCompany);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
