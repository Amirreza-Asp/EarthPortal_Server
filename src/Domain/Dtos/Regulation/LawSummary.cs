namespace Domain.Dtos.Regulation
{
    public class LawSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Type { get; set; }
        public String ApprovalAuthority { get; set; }
        public DateTime ApprovalDate { get; set; }
        public String Pdf { get; set; }
    }
}
