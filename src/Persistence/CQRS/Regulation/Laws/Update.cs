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
        private readonly IHostingEnvironment _env;
        private readonly IFileManager _fileManager;
        private readonly ILogger<UpdateLawCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateLawCommandHandler(ApplicationDbContext context, IFileManager fileManager, IHostingEnvironment env, ILogger<UpdateLawCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _fileManager = fileManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateLawCommand request, CancellationToken cancellationToken)
        {
            var law = await _context.Law.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            var upload = _env.WebRootPath;

            if (law == null)
                return CommandResponse.Failure(400, "قانون انتخاب شده در سیستم وجود ندارد");

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            String oldFileName = "";
            String newFileName = "";

            if (request.Pdf != null)
            {
                newFileName = Guid.NewGuid() + Path.GetExtension(request.Pdf.FileName);
                oldFileName = law.Pdf;

                law.Pdf = newFileName;

                await _fileManager.SaveFileAsync(request.Pdf, upload + SD.LawPdfPath + newFileName);
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
            law.Announcement = new Announcement(request.AnnouncementNumber, request.AnnouncementDate);
            law.Newspaper = Newspaper.Create(request.NewspaperNumber, request.NewspaperDate);
            law.Type = request.Type == 0 ? LawType.Rule : LawType.Regulation;

            _context.Law.Update(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Pdf != null)
                {
                    if (File.Exists(upload + SD.LawPdfPath + oldFileName))
                        File.Delete(upload + SD.LawPdfPath + oldFileName);
                }


                _logger.LogInformation($"Law with id {law.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
