using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Articles;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Articles
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateArticleCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateArticleCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, ILogger<UpdateArticleCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Article.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "مقاله مورد نظر در سیستم وجود ندارد");

            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.ArticleImagePath))
                Directory.CreateDirectory(upload + SD.ArticleImagePath);

            if (!Directory.Exists(upload + SD.ArticleFilePath))
                Directory.CreateDirectory(upload + SD.ArticleFilePath);


            var oldImage = entity.Image;
            var oldFile = entity.File;

            entity.Order = request.Order;
            entity.AuthorId = request.AuthorId;
            entity.ShortDescription = request.ShortDescription;
            entity.Description = request.Description;
            entity.Title = request.Title;

            entity.PublicationId = request.PublicationId;
            entity.Pages = request.Pages;
            entity.PublishDate = request.PublishDate;
            entity.TranslatorId = request.TranslatorId;


            if (request.File != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
                entity.File = fileName;

                using (Stream fileStream = new FileStream(upload + SD.ArticleFilePath + entity.File, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
            }

            if (request.Image != null)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                entity.Image = imgName;


                _photoManager.Save(request.Image, upload + SD.ArticleImagePath + entity.Image);
            }

            _context.Article.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                if (request.Image != null)
                {
                    if (File.Exists(upload + SD.ArticleImagePath + oldImage))
                        File.Delete(upload + SD.ArticleImagePath + oldImage);
                }

                if (request.File != null)
                {
                    if (File.Exists(upload + SD.ArticleFilePath + oldFile))
                        File.Delete(upload + SD.ArticleFilePath + oldFile);
                }


                _logger.LogInformation($"Article with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");

                return CommandResponse.Success(new { Id = entity.Id, Image = entity.Image, File = entity.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
