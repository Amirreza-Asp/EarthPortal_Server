using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class Law : BaseEntity
    {
        public Law(string title, Announcement announcement, Newspaper newspaper, string description, DateTime approvalDate, LawType type, bool isOriginal, Guid approvalTypeId, Guid approvalStatusId, Guid executorManagmentId, Guid approvalAuthorityId, Guid lawCategoryId, string pdf)
        {
            Title = title;
            Announcement = announcement;
            Newspaper = newspaper;
            Description = description;
            ApprovalDate = approvalDate;
            Type = type;
            IsOriginal = isOriginal;
            ApprovalTypeId = approvalTypeId;
            ApprovalStatusId = approvalStatusId;
            ExecutorManagmentId = executorManagmentId;
            ApprovalAuthorityId = approvalAuthorityId;
            LawCategoryId = lawCategoryId;
            Pdf = pdf;
        }

        private Law() { }

        public String Title { get; set; }
        public Announcement Announcement { get; set; }
        public Newspaper Newspaper { get; set; }
        public String Description { get; set; }
        public DateTime ApprovalDate { get; set; }
        public LawType Type { get; set; }
        public bool IsOriginal { get; set; } = true;
        public String Pdf { get; set; }

        public Guid ApprovalTypeId { get; set; }
        public Guid ApprovalStatusId { get; set; }
        public Guid ExecutorManagmentId { get; set; }
        public Guid ApprovalAuthorityId { get; set; }
        public Guid LawCategoryId { get; set; }



        public ApprovalType? ApprovalType { get; set; }
        public ApprovalStatus? ApprovalStatus { get; set; }
        public ExecutorManagment? ExecutorManagment { get; set; }
        public ApprovalAuthority? ApprovalAuthority { get; set; }
        public LawCategory? LawCategory { get; set; }

    }
}
