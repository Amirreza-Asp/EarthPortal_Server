using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Notices;
using Application.Models;
using Domain;
using Domain.Entities.Notices;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Notices
{
    public class UpdateNoticeCommandHandler : IRequestHandler<UpdateNoticeCommand, CommandResponse>
    {

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateNoticeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateNoticeCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, IMediator mediator, ILogger<UpdateNoticeCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _mediator = mediator;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateNoticeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notice = await _context.Notices.Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

                if (notice == null)
                    return CommandResponse.Failure(400, "اطلاعیه مورد نظر در سیستم وجود ندارد");

                _context.Database.BeginTransaction();

                notice.Order = request.Order;
                notice.DateOfRegisration = request.DateOfRegisration;
                notice.Description = request.Description;
                notice.Title = request.Title;
                notice.Headline = request.Headline;
                notice.Source = request.Source;
                notice.IsActive = request.IsActive;

                _context.Notices.Update(notice);

                var upsertNoticeLink = new UpsertNoticeLinkCommand(request.Links == null ? new List<string>() : request.Links, request.Id);
                var response = await _mediator.Send(upsertNoticeLink, cancellationToken);

                if (response.Status != 200)
                    return response;

                var image = await _context.NoticeImage.Where(b => b.NoticeId == request.Id).FirstOrDefaultAsync(cancellationToken);

                var oldImage = image != null ? image.Name : String.Empty;

                var upload = _env.WebRootPath + SD.NoticeImagePath;

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                if (request.Image != null)
                {
                    if (image != null)
                    {
                        image.Name = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
                        _context.NoticeImage.Update(image);
                    }
                    else
                    {
                        image = new NoticeImage(Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName), request.Id, 0);
                        _context.NoticeImage.Add(image);
                    }

                    _photoManager.Save(request.Image, upload + image.Name);
                }

                await _context.SaveChangesAsync(cancellationToken);

                if (request.Image != null && !String.IsNullOrEmpty(oldImage) && File.Exists(upload + oldImage))
                    File.Delete(upload + oldImage);

                _context.Database.CommitTransaction();
                _logger.LogInformation($"Notice with id {notice.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success(new { Image = image.Name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                _context.Database.RollbackTransaction();
                return CommandResponse.Failure(400, "عملیات با خطا مواجه شد");
            }
        }
    }
}
