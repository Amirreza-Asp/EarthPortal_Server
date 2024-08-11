namespace Application.Queries
{
    public class LawPagenationQuery
    {
        public String? Text { get; set; }

        public List<String> SearchProps { get; set; }
        public List<int>? LawType { get; set; }

        public List<Guid>? ApprovalAuthorityIds { get; set; }
        public List<Guid>? ExecutorManagmentIds { get; set; }
        public List<Guid>? ApprovalStatusIds { get; set; }
        public List<Guid>? LawCategoryIds { get; set; }
        public List<Guid>? ApprovalTypeIds { get; set; }

        public int IsOriginal { get; set; } = -1;
        public DateTime? ApprovalDate { get; set; }

        public String? AnnouncementNumber { get; set; }
        public DateTime? AnnouncementDate { get; set; }
        public String? NewspaperNumber { get; set; }
        public DateTime? NewspaperDate { get; set; }

        public String? SortBy { get; set; }
        public bool ascSort { get; set; }

        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;

        public bool IsFiltered() =>
            ApprovalAuthorityIds != null && ApprovalAuthorityIds.Any() ||
            ExecutorManagmentIds != null && ExecutorManagmentIds.Any() ||
            ApprovalStatusIds != null && ApprovalStatusIds.Any() ||
            LawCategoryIds != null && LawCategoryIds.Any() ||
            ApprovalTypeIds != null && ApprovalTypeIds.Any() ||
            !String.IsNullOrEmpty(AnnouncementNumber) ||
            AnnouncementDate.HasValue ||
             !String.IsNullOrEmpty(NewspaperNumber) ||
            NewspaperDate.HasValue ||
            ApprovalDate.HasValue;
    }
}
