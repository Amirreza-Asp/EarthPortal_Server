using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pages
{
    public class FooterPage
    {
        public FooterPage(int todaySeen, long totalSeen, int todayTotalSeen)
        {
            Id = Guid.NewGuid();
            TodaySeen = todaySeen;
            TotalSeen = totalSeen;
            TodayTotalSeen = todayTotalSeen;
        }

        [Key]
        public Guid Id { get; set; }

        public int TodaySeen { get; set; }
        public long TotalSeen { get; set; }
        public int TodayTotalSeen { get; set; }
    }
}
