﻿using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Books;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Books
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPhotoManager _photoManager;
        private readonly ILogger<UpdateBookCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateBookCommandHandler(ApplicationDbContext context, IHostingEnvironment env, IPhotoManager photoManager, ILogger<UpdateBookCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _photoManager = photoManager;
            _logger = logger;
            _userAccessor = userAccessor;
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




            var oldImage = book.Image;
            var oldFile = book.File;

            book.Order = request.Order;
            book.AuthorId = request.AuthorId;
            book.ShortDescription = request.ShortDescription;
            book.Description = request.Description;
            book.Title = request.Title;

            book.PublicationId = request.PublicationId;
            book.Pages = request.Pages;
            book.PublishDate = request.PublishDate;
            book.TranslatorId = request.TranslatorId;


            if (request.File != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
                book.File = fileName;

                using (Stream fileStream = new FileStream(upload + SD.BookFilePath + book.File, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
            }

            if (request.Image != null)
            {
                var imgName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                book.Image = imgName;
                _photoManager.Save(request.Image, upload + SD.BookImagePath + book.Image);
            }

            _context.Book.Update(book);


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                if (request.Image != null)
                {
                    if (File.Exists(upload + SD.BookImagePath + oldImage))
                        File.Delete(upload + SD.BookImagePath + oldImage);

                }

                if (request.File != null)
                {
                    if (File.Exists(upload + SD.BookFilePath + oldFile))
                        File.Delete(upload + SD.BookFilePath + oldFile);

                }

                _logger.LogInformation($"Book with id {book.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(new { Id = book.Id, Image = book.Image, File = book.File });
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
