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
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMediator _mediator;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateNewsCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;


        public CreateNewsCommandHandler(ApplicationDbContext context, IMediator mediator, IPhotoManager photoManager, IHostingEnvironment env, ILogger<CreateNewsCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _mediator = mediator;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            var shortLink = CreateRandomLink();

            while (await _context.News.AnyAsync(b => b.ShortLink == shortLink))
                shortLink = CreateRandomLink();


            var category = await _context.NewsCategory.FirstOrDefaultAsync(b => b.Title == "نامشخص");
            if (category == null)
            {
                category = new NewsCategory("نامشخص", null);
                _context.NewsCategory.Add(category);
            }

            var news = new News(request.Title, request.Description, request.Headline, request.Source, request.DateOfRegisration, category.Id, shortLink);
            news.Order = request.Order;
            _context.News.Add(news);

            var links = request.Links?.Select(link => new Link(link));

            if (request.Links != null && request.Links.Any())
                await _mediator.Send(new UpsertNewsLinkCommand(request.Links, news.Id));


            var image = new NewsImage(Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName), news.Id, 0);
            _context.Add(image);

            var upload = _env.WebRootPath + SD.NewsImagePath;
            if (!Directory.Exists(upload))
                Directory.CreateDirectory(upload);

            _photoManager.Save(request.Image, upload + image.Name);
            if (await _context.SaveChangesAsync() > 0)
            {

                _logger.LogInformation($"News with id {news.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = news.Id, Image = news.Images.First().Name, ShortLink = news.ShortLink });
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
