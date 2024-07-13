using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Books;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Books
{
    public class RemoveBookCommandHandler : IRequestHandler<RemoveBookCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<RemoveBookCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveBookCommandHandler(ApplicationDbContext context, IHostingEnvironment env, ILogger<RemoveBookCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == request.Id);
            var upload = _env.WebRootPath;

            if (book == null)
                return CommandResponse.Failure(400, "کتاب مورد نظر در سیستم وجود ندارد");

            if (File.Exists(upload + SD.BookFilePath + book.File))
                File.Delete(upload + SD.BookFilePath + book.File);

            if (File.Exists(upload + SD.BookImagePath + book.Image))
                File.Delete(upload + SD.BookImagePath + book.Image);

            _context.Book.Remove(book);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Book with id {book.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
