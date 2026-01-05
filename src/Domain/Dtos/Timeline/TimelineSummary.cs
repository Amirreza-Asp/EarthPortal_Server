

namespace Domain.Dtos.Timeline;

public class TimelineSummary
{
    public Guid Id { get; set; }
    public String Title { get; set; }
    public String Content { get; set; }
    public String? Image { get; set; }
    public String? Video { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime AccomplishedDate { get; set; }
    public bool HaveVideo { get; set; }
}
