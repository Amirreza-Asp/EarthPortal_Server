using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Books;
using Application.Models;
using Domain;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Persistence.CQRS.Resources.Books
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;

        public CreateBookCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
        }

        public async Task<CommandResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var upload = _env.WebRootPath;

            if (!Directory.Exists(upload + SD.BookImagePath))
                Directory.CreateDirectory(upload + SD.BookImagePath);

            if (!Directory.Exists(upload + SD.BookFilePath))
                Directory.CreateDirectory(upload + SD.BookFilePath);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
            var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);


            var book = new Book(request.Title, request.Description, fileName, request.PublishDate, request.AuthorId, imgName, request.ShortDescription, request.Pages, request.TranslatorId, request.PublicationId);

            _context.Book.Add(book);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
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
