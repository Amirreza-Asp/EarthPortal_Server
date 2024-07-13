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
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, CommandResponse>
    {

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateNewsCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateNewsCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, IMediator mediator, ILogger<UpdateNewsCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _mediator = mediator;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {

            var news = await _context.News.Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (news == null)
                return CommandResponse.Failure(400, "خبر مورد نظر در سیستم وجود ندارد");

            news.Order = request.Order;
            news.DateOfRegisration = request.DateOfRegisration;
            news.Description = request.Description;
            news.Title = request.Title;
            news.Headline = request.Headline;
            news.Source = request.Source;

            var upsertNewsLink = new UpsertNewsLinkCommand(request.Links == null ? new List<string>() : request.Links, request.Id);
            var response = await _mediator.Send(upsertNewsLink, cancellationToken);


            if (response.Status != 200)
                return response;

            var image = await _context.NewsImage.Where(b => b.NewsId == request.Id).FirstOrDefaultAsync(cancellationToken);

            var oldImage = image != null ? image.Name : String.Empty;

            var upload = _env.WebRootPath + SD.NewsImagePath;
            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            if (request.Image != null)
            {
                if (image != null)
                {
                    image.Name = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
                    _context.NewsImage.Update(image);
                }
                else
                {
                    image = new NewsImage(Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName), request.Id, 0);
                    _context.NewsImage.Add(image);
                }


                _photoManager.Save(request.Image, upload + image.Name);
            }



            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (request.Image != null && !String.IsNullOrEmpty(oldImage) && File.Exists(upload + oldImage))
                    File.Delete(upload + oldImage);


                _logger.LogInformation($"News with id {news.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success(new { Image = image.Name });
            }

            return CommandResponse.Success();
        }
    }
}
