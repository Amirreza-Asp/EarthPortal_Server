using Application.CQRS.Resources.Articles;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Articles
{
    public class RemoveArticleCommandHandler : IRequestHandler<RemoveArticleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public RemoveArticleCommandHandler(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<CommandResponse> Handle(RemoveArticleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Article.FirstOrDefaultAsync(b => b.Id == request.Id);
            var upload = _env.WebRootPath;

            if (entity == null)
                return CommandResponse.Failure(400, "مقاله مورد نظر در سیستم وجود ندارد");

            if (File.Exists(upload + SD.ArticleFilePath + entity.File))
                File.Delete(upload + SD.ArticleFilePath + entity.File);

            if (File.Exists(upload + SD.ArticleImagePath + entity.Image))
                File.Delete(upload + SD.ArticleImagePath + entity.Image);

            _context.Article.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
