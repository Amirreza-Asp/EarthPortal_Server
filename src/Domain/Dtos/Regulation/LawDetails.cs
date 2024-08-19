using Domain.Entities.Regulation.ValueObjects;

namespace Domain.Dtos.Regulation
{
    public class LawDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Type { get; set; }
        public DateTime ApprovalDate { get; set; }
        public String ApprovalAuthority { get; set; }
        public Announcement Announcement { get; set; }
        public Newspaper Newspaper { get; set; }
        public bool IsOriginal { get; set; }
        public String Pdf { get; set; }
        public String Article { get; set; }

        public Guid ApprovalTypeId { get; set; }
        public string ApprovalTypeTitle { get; set; }
        public Guid ApprovalStatusId { get; set; }
        public string ApprovalStatusTitle { get; set; }
        public Guid ExecutorManagmentId { get; set; }
        public string ExecutorManagmentTitle { get; set; }
        public Guid ApprovalAuthorityId { get; set; }
        public string ApprovalAuthorityTitle { get; set; }
        public Guid LawCategoryId { get; set; }
        public string LawCategoryTitle { get; set; }
        public int Order { get; set; }

    }
}
