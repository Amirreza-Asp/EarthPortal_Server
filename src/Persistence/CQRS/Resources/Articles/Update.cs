using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Articles;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Articles
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public UpdateArticleCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
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
            }

            if (request.Image != null)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                entity.Image = imgName;
            }

            _context.Article.Update(entity);


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                if (request.Image != null)
                {
                    if (File.Exists(upload + SD.ArticleImagePath + oldImage))
                        File.Delete(upload + SD.ArticleImagePath + oldImage);

                    await _photoManager.SaveAsync(request.Image, upload + SD.ArticleImagePath + entity.Image, cancellationToken);
                }

                if (request.File != null)
                {
                    if (File.Exists(upload + SD.ArticleFilePath + oldFile))
                        File.Delete(upload + SD.ArticleFilePath + oldFile);

                    using (Stream fileStream = new FileStream(upload + SD.ArticleFilePath + entity.File, FileMode.Create))
                    {
                        await request.File.CopyToAsync(fileStream);
                    }
                }

                return CommandResponse.Success(new { Id = entity.Id, Image = entity.Image, File = entity.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
