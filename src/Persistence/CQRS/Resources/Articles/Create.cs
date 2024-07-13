using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Articles;
using Application.Models;
using Domain;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Articles
{
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateArticleCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateArticleCommandHandler(ApplicationDbContext context, IPhotoManager photoManager, IHostingEnvironment env, ILogger<CreateArticleCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _photoManager = photoManager;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.ArticleFilePath))
                Directory.CreateDirectory(upload + SD.ArticleFilePath);

            if (!Directory.Exists(upload + SD.ArticleImagePath))
                Directory.CreateDirectory(upload + SD.ArticleImagePath);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            using (Stream fileStream = new FileStream(upload + SD.ArticleFilePath + fileName, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            _photoManager.Save(request.Image, upload + SD.ArticleImagePath + imgName);

            var entity = new Article(request.Title, request.Description, fileName, request.PublishDate, request.AuthorId, imgName, request.ShortDescription, request.Pages, request.TranslatorId, request.PublicationId);
            entity.Order = request.Order;
            _context.Article.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Article with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = entity.Id, Image = entity.Image, File = entity.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
