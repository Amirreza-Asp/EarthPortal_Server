using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Regulation.Laws
{
    public class CreateLawCommandHandler : IRequestHandler<CreateLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostEnv;
        private readonly IFileManager _fileManager;

        public CreateLawCommandHandler(ApplicationDbContext context, IHostingEnvironment hostEnv, IFileManager fileManager)
        {
            _context = context;
            _hostEnv = hostEnv;
            _fileManager = fileManager;
        }

        public async Task<CommandResponse> Handle(CreateLawCommand request, CancellationToken cancellationToken)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(request.Pdf.FileName);
            var upload = _hostEnv.WebRootPath + SD.LawPdfPath;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);



            var law = new Law(request.Title, new Announcement(request.AnnouncementNumber, request.AnnouncementDate), Newspaper.Create(request.AnnouncementNumber, request.NewspaperDate),
                             request.Description, request.ApprovalDate, request.Type == 0 ? LawType.Rule : LawType.Regulation, request.IsOriginal, request.ApprovalTypeId, request.ApprovalStatusId,
                             request.ExecutorManagmentId, request.ApprovalAuthorityId, request.LawCategoryId, fileName);

            law.Order = request.Order;
            _context.Law.Add(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                await _fileManager.SaveFileAsync(request.Pdf, upload + fileName);
                return CommandResponse.Success(law.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
