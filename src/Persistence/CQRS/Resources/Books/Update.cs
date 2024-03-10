using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Resources;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Resources.Books
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public UpdateBookCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
        }

        public async Task<CommandResponse> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (book == null)
                return CommandResponse.Failure(400, "کتاب مورد نظر در سیستم وجود ندارد");

            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.BookImagePath))
                Directory.CreateDirectory(upload + SD.BookImagePath);

            if (!Directory.Exists(upload + SD.BookFilePath))
                Directory.CreateDirectory(upload + SD.BookFilePath);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
            var oldImage = book.Image;
            var oldFile = book.File;


            book.AuthorId = request.AuthorId;
            book.ShortDescription = request.ShortDescription;
            book.Description = request.Description;
            book.Title = request.Title;
            book.File = fileName;
            book.Image = imgName;
            book.PublicationId = request.PublicationId;
            book.Pages = request.Pages;
            book.PublishDate = request.PublishDate;
            book.TranslatorId = request.TranslatorId;

            _context.Book.Update(book);


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                if (File.Exists(upload + SD.BookImagePath + oldImage))
                    File.Delete(upload + SD.BookImagePath + oldImage);

                if (File.Exists(upload + SD.BookFilePath + oldFile))
                    File.Delete(upload + SD.BookFilePath + oldFile);


                using (Stream fileStream = new FileStream(upload + SD.BookFilePath + fileName, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                await _photoManager.SaveAsync(request.Image, upload + SD.BookImagePath + imgName, cancellationToken);

                return CommandResponse.Success(new { Id = book.Id, Image = book.Image, File = book.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
