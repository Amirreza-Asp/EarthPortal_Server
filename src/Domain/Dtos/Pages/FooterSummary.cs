namespace Domain.Dtos.Pages
{
    public class FooterSummary
    {
        public long TodaySeen { get; set; }
        public long TotalSeen { get; set; }
        public long TodayTotalSeen { get; set; }
        public long OnlineUsers { get; set; }
        public String Ip { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
