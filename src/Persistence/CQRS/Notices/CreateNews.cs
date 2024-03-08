using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Notices;
using Application.Models;
using Domain;
using Domain.Entities.Notices;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;

        public CreateNewsCommandHandler(ApplicationDbContext context, IMediator mediator, IPhotoManager photoManager, IHostingEnvironment env)
        {
            _context = context;
            _mediator = mediator;
            _photoManager = photoManager;
            _env = env;
        }

        public async Task<CommandResponse> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            if (request.Links == null || request.Links.Count == 0)
                return CommandResponse.Failure(400, "کلیدواژه ها را وارد کنید");

            var shortLink = CreateRandomLink();

            while (await _context.News.AnyAsync(b => b.ShortLink == shortLink))
                shortLink = CreateRandomLink();

            var news = new News(request.Title, request.Description, request.Headline, request.Source, request.DateOfRegisration, request.NewsCategoryId, shortLink);

            _context.News.Add(news);

            var links = request.Links.Select(link => new Link(link));

            await _mediator.Send(new UpsertNewsLinkCommand(request.Links, news.Id));


            var image = new NewsImage(Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName), news.Id, 0);
            _context.Add(image);

            if (await _context.SaveChangesAsync() > 0)
            {
                var upload = _env.WebRootPath + SD.NewsImagePath;

                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);

                await _photoManager.SaveAsync(request.Image, upload + image.Name, cancellationToken);

                return CommandResponse.Success(new { Id = news.Id, Image = news.Images.First().Name });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }


        private int CreateRandomLink()
        {
            var rnd = new Random();
            return rnd.Next(Convert.ToInt32(Math.Pow(10, 7)), Convert.ToInt32(Math.Pow(10, 8)));
        }
    }
}
