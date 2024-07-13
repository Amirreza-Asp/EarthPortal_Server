using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Books;
using Application.Models;
using Domain;
using Domain.Entities.Resources;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Books
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<CreateBookCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateBookCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, ILogger<CreateBookCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _logger = logger;
            _userAccessor = userAccessor;
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

            book.Order = request.Order;
            _context.Book.Add(book);


            _photoManager.Save(request.Image, upload + SD.BookImagePath + imgName);
            using (Stream fileStream = new FileStream(upload + SD.BookFilePath + fileName, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {



                _logger.LogInformation($"Book with id {book.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = book.Id, Image = book.Image, File = book.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
