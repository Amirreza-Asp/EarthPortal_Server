using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Laws
{
    public class UpdateLawCommandHandler : IRequestHandler<UpdateLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IFileManager _fileManager;
        private readonly ILogger<UpdateLawCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateLawCommandHandler(
            ApplicationDbContext context,
            IFileManager fileManager,
            IWebHostEnvironment env,
            ILogger<UpdateLawCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _fileManager = fileManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdateLawCommand request,
            CancellationToken cancellationToken
        )
        {
            var law = await _context
                .Law.Include(x => x.LawLawContents)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            var pdfUpload = _env.WebRootPath + SD.LawPdfPath;
            var newspaperUpload = _env.WebRootPath + SD.LawNewspaperPath;

            if (law == null)
                return CommandResponse.Failure(400, "قانون انتخاب شده در سیستم وجود ندارد");

            if (!Directory.Exists(pdfUpload))
                Directory.CreateDirectory(pdfUpload);

            if (!Directory.Exists(newspaperUpload))
                Directory.CreateDirectory(newspaperUpload);

            String oldFileName = "";
            String newFileName = "";

            if (request.Pdf != null)
            {
                newFileName = Guid.NewGuid() + Path.GetExtension(request.Pdf.FileName);
                oldFileName = law.Pdf;

                law.Pdf = newFileName;

                await _fileManager.SaveFileAsync(request.Pdf, pdfUpload + newFileName);
            }

            var newspaperFile = String.Empty;
            var oldNewspaperFileName = law?.Newspaper?.File;

            if (request.NewspaperFile != null)
            {
                newspaperFile = Guid.NewGuid() + Path.GetExtension(request.NewspaperFile.FileName);
                await _fileManager.SaveFileAsync(
                    request.NewspaperFile,
                    newspaperUpload + newspaperFile
                );
            }

            law.Order = request.Order;
            law.ApprovalDate = request.ApprovalDate;
            law.ApprovalStatusId = request.ApprovalStatusId;
            law.ApprovalAuthorityId = request.ApprovalAuthorityId;
            law.ApprovalTypeId = request.ApprovalTypeId;
            law.ExecutorManagmentId = request.ExecutorManagmentId;
            law.LawCategoryId = request.LawCategoryId;
            law.Title = request.Title;
            law.Description = request.Description;
            law.IsOriginal = request.IsOriginal;
            law.Announcement = new Announcement(
                request.AnnouncementNumber,
                request.AnnouncementDate
            );
            law.Newspaper = Newspaper.Create(
                request.NewspaperNumber,
                request.NewspaperDate,
                newspaperFile
            );
            law.Type = request.Type == 0 ? LawType.Rule : LawType.Regulation;
            law.LastModifiedAt = DateTime.Now;

            var requestedIds = request.LawContentIds.Distinct().ToHashSet();

            var toRemove = law
                .LawLawContents.Where(x => !requestedIds.Contains(x.LawContentId))
                .ToList();

            foreach (var item in toRemove)
            {
                law.LawLawContents.Remove(item);
            }

            var existingIds = law.LawLawContents.Select(x => x.LawContentId).ToHashSet();

            foreach (var id in requestedIds.Except(existingIds))
            {
                law.LawLawContents.Add(new Domain.Entities.Regulation.LawLawContent(law.Id, id));
            }

            //_context.Law.Update(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Pdf != null)
                {
                    if (File.Exists(pdfUpload + SD.LawPdfPath + oldFileName))
                        File.Delete(pdfUpload + SD.LawPdfPath + oldFileName);
                }

                if (
                    request.NewspaperFile != null
                    && File.Exists(SD.LawPdfPath + oldNewspaperFileName)
                )
                    File.Delete(SD.LawPdfPath + oldNewspaperFileName);

                _logger.LogInformation(
                    $"Law with id {law.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
