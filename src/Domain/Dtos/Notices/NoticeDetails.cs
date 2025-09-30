namespace Domain.Dtos.Notices
{
    public class NoticeDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Image { get; set; }
        public String Headline { get; set; }
        public String Description { get; set; }
        public long Seen { get; set; }
        public String Source { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public Guid NoticeCategoryId { get; set; }
        public List<Keyword> Keywords { get; set; }
        public List<NoticeSummary> RelatedNotices { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }


        public NoticeSummary? NextNotice { get; set; }
        public NoticeSummary? PrevNotice { get; set; }
    }



}
