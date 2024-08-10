using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Laws
{
    public class CreateLawCommandHandler : IRequestHandler<CreateLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostEnv;
        private readonly IFileManager _fileManager;
        private readonly ILogger<CreateLawCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateLawCommandHandler(ApplicationDbContext context, IHostingEnvironment hostEnv, IFileManager fileManager, ILogger<CreateLawCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _hostEnv = hostEnv;
            _fileManager = fileManager;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateLawCommand request, CancellationToken cancellationToken)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(request.Pdf.FileName);
            var upload = _hostEnv.WebRootPath + SD.LawPdfPath;

            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            await _fileManager.SaveFileAsync(request.Pdf, upload + fileName);


            var law = new Law(request.Title, new Announcement(request.AnnouncementNumber, request.AnnouncementDate), Newspaper.Create(request.AnnouncementNumber, request.NewspaperDate),
                             request.Description, request.ApprovalDate, request.Type == 0 ? LawType.Rule : LawType.Regulation, request.IsOriginal, request.ApprovalTypeId, request.ApprovalStatusId,
                             request.ExecutorManagmentId, request.ApprovalAuthorityId, request.LawCategoryId, fileName, request.Article);

            law.Order = request.Order;
            _context.Law.Add(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"Law with id {law.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(law.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
