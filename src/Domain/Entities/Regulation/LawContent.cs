using Domain.Shared;

namespace Domain.Entities.Regulation;

public class LawContent : BaseEntity
{
    public LawContent(string title)
        : base(Guid.NewGuid())
    {
        Title = title;
    }

    public string Title { get; set; }

    public ICollection<LawLawContent> LawLawContents { get; private set; } =
        new List<LawLawContent>();
}
