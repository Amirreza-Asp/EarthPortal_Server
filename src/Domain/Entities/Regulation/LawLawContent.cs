namespace Domain.Entities.Regulation;

public class LawLawContent
{
    public Guid LawId { get; private set; }
    public Law Law { get; private set; } = null!;

    public Guid LawContentId { get; private set; }
    public LawContent LawContent { get; private set; } = null!;

    private LawLawContent() { }

    public LawLawContent(Guid lawId, Guid lawContentId)
    {
        LawId = lawId;
        LawContentId = lawContentId;
    }
}
