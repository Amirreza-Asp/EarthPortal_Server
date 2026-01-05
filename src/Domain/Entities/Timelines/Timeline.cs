using Domain.Shared;

namespace Domain.Entities.Timelines;

public class Timeline : BaseEntity
{
    public Timeline(
        string title,
        string content,
        string? video,
        string? image,
        DateTime accomplishedDate
    )
        : base(Guid.NewGuid())
    {
        Title = title;
        Content = content;
        Video = video;
        Image = image;
        AccomplishedDate = accomplishedDate;
    }

    private Timeline() { }

    public string Title { get; set; }
    public string Content { get; set; }
    public string? Video { get; set; }
    public string? Image { get; set; }
    public DateTime AccomplishedDate { get; set; }
}
