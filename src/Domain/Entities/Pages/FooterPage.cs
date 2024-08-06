using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pages
{
    public class FooterPage
    {
        public FooterPage(Guid id)
        {
            Id = id;
            LastUpdate = DateTime.Now;
        }

        private FooterPage() { }

        [Key]
        public Guid Id { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
