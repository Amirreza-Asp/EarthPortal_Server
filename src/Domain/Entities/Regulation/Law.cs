using Domain.Entities.Regulation.ValueObjects;
using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class Law : BaseEntity
    {
        public Law(string title, Announcement announcement, Newspaper newspaperNumber, string description, DateTime approvalDate, Guid approvalTypeId, Guid executorManagmentId, Guid approvalAuthorityId, Guid lawCategoryId)
        {
            Title = title;
            Announcement = announcement;
            NewspaperNumber = newspaperNumber;
            Description = description;
            ApprovalDate = approvalDate;
            ApprovalTypeId = approvalTypeId;
            ExecutorManagmentId = executorManagmentId;
            ApprovalAuthorityId = approvalAuthorityId;
            LawCategoryId = lawCategoryId;
        }

        private Law() { }

        public String Title { get; set; }
        public Announcement Announcement { get; set; }
        public Newspaper NewspaperNumber { get; set; }
        public String Description { get; set; }
        public DateTime ApprovalDate { get; set; }
        public Guid ApprovalTypeId { get; set; }
        public Guid ExecutorManagmentId { get; set; }
        public Guid ApprovalAuthorityId { get; set; }
        public Guid LawCategoryId { get; set; }


        public ApprovalType ApprovalType { get; set; }
        public ExecutorManagment ExecutorManagment { get; set; }
        public ApprovalAuthority ApprovalAuthority { get; set; }
        public LawCategory LawCategory { get; set; }

    }
}
