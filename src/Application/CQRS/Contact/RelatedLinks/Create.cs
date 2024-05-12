using Application.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.CQRS.Contact.RelatedLinks
{
    public class CreateRelatedLinkCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String Link { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
