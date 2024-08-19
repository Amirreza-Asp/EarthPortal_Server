using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Regulation.Laws
{
    public class UpdateLawCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public string AnnouncementNumber { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public string NewspaperNumber { get; set; }
        public DateTime NewspaperDate { get; set; }
        public IFormFile? NewspaperFile { get; set; }
        public String Description { get; set; }
        public DateTime ApprovalDate { get; set; }
        public bool IsOriginal { get; set; }
        public int Type { get; set; }

        public IFormFile? Pdf { get; set; }

        public Guid ApprovalTypeId { get; set; }
        public Guid ApprovalStatusId { get; set; }
        public Guid ExecutorManagmentId { get; set; }
        public Guid ApprovalAuthorityId { get; set; }
        public Guid LawCategoryId { get; set; }
        public int Order { get; set; }
    }
}
