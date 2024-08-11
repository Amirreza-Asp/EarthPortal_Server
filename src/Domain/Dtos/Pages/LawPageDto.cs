namespace Domain.Dtos.Pages
{
    public class LawPageDto
    {
        public Guid Id { get; set; }

        public String WarningTitle { get; set; }
        public String WarningContent { get; set; }
        public int LawCount { get; set; }
    }
}
