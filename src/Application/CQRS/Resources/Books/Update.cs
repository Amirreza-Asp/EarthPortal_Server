using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Resources.Books
{
    public class UpdateBookCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? File { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid PublicationId { get; set; }
        public Guid AuthorId { get; set; }
        public int Pages { get; set; }
        public Guid TranslatorId { get; set; }
    }
}
