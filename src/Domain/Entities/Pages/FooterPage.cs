using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pages
{
    public class FooterPage
    {
        public FooterPage(Guid id)
        {
            Id = id;
            TodaySeen = 1;
            TotalSeen = 1;
            TodayTotalSeen = 1;
            LastUpdate = DateTime.Now;
            OnlineUsers = 1;
            Today = DateTime.Now;
        }

        private FooterPage() { }

        [Key]
        public Guid Id { get; set; }

        public int TodaySeen { get; set; }
        public long TotalSeen { get; set; }
        public int TodayTotalSeen { get; set; }
        public int OnlineUsers { get; set; }
        public DateTime Today { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
