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
    public class CreateNoticeCommandHandler : IRequestHandler<CreateNoticeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateNoticeCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateNoticeCommandHandler(
            ApplicationDbContext context,
            IMediator mediator,
            IPhotoManager photoManager,
            IWebHostEnvironment env,
            ILogger<CreateNoticeCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _mediator = mediator;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateNoticeCommand request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                _context.Database.BeginTransaction();

                var shortLink = CreateRandomLink();

                while (await _context.Notices.AnyAsync(b => b.ShortLink == shortLink))
                    shortLink = CreateRandomLink();

                var category = await _context.NoticeCategory.FirstOrDefaultAsync(b =>
                    b.Title == "نامشخص"
                );
                if (category == null)
                {
                    category = new NoticeCategory("نامشخص", null);
                    _context.NoticeCategory.Add(category);
                }

                var notice = new Notice(
                    request.Title,
                    request.Description,
                    request.Headline,
                    request.Source,
                    request.DateOfRegisration,
                    category.Id,
                    shortLink,
                    request.IsActive
                );
                var image = new NoticeImage(
                    Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName),
                    notice.Id,
                    0
                );

                var upload = _env.WebRootPath + SD.NoticeImagePath;
                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                _photoManager.Save(request.Image, upload + image.Name);

                notice.Order = request.Order;
                _context.Notices.Add(notice);
                _context.Add(image);

                var links = request.Links?.Select(link => new Link(Guid.NewGuid(), link));

                if (request.Links != null && request.Links.Any())
                    await _mediator.Send(new UpsertNoticeLinkCommand(request.Links, notice.Id));

                await _context.SaveChangesAsync();

                _context.Database.CommitTransaction();

                _logger.LogInformation(
                    $"Notice with id {notice.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}"
                );
                return CommandResponse.Success(
                    new
                    {
                        Id = notice.Id,
                        Image = notice.Images.First().Name,
                        ShortLink = notice.ShortLink
                    }
                );
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                _logger.LogError(ex.Message, ex);
                return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
            }
        }

        private int CreateRandomLink()
        {
            var rnd = new Random();
            return rnd.Next(Convert.ToInt32(Math.Pow(10, 7)), Convert.ToInt32(Math.Pow(10, 8)));
        }
    }
}
