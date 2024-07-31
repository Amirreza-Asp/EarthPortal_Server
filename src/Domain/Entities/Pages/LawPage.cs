using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pages
{
    public class LawPage
    {
        public LawPage(string warningTitle, string warningContent)
        {
            Id = Guid.NewGuid();
            WarningTitle = warningTitle;
            WarningContent = warningContent;
        }

        private LawPage() { }

        [Key]
        public Guid Id { get; set; }

        public String WarningTitle { get; set; }
        public String WarningContent { get; set; }
    }
}
