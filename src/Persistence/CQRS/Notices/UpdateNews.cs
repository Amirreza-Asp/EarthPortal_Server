using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Notices;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, CommandResponse>
    {

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;

        public UpdateNewsCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, IMediator mediator)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _mediator = mediator;
        }

        public async Task<CommandResponse> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            if (request.Links == null || request.Links.Count == 0)
                return CommandResponse.Failure(400, "کلیدواژه ها را وارد کنید");


            var news = await _context.News.Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (news == null)
                return CommandResponse.Failure(400, "خبر مورد نظر در سیستم وجود ندارد");

            news.DateOfRegisration = request.DateOfRegisration;
            news.Description = request.Description;
            news.Title = request.Title;
            news.Headline = request.Headline;
            news.Source = request.Source;
            news.NewsCategoryId = request.NewsCategoryId;
            _context.News.Update(news);

            var upsertNewsLink = new UpsertNewsLinkCommand(request.Links, request.Id);
            var response = await _mediator.Send(upsertNewsLink, cancellationToken);

            if (response.Status != 200)
                return response;

            var image = await _context.NewsImage.Where(b => b.NewsId == request.Id).FirstAsync(cancellationToken);
            if (request.Image != null)
            {

                image.Name = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
                _context.NewsImage.Update(image);

                var upload = _env.WebRootPath + SD.NewsImagePath;

                if (image != null && File.Exists(upload + image.Name))
                    File.Delete(upload + image.Name);

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                await _photoManager.SaveAsync(request.Image, upload + image.Name, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return CommandResponse.Success(new { Image = image.Name });
            }

            return CommandResponse.Success();
        }
    }
}
