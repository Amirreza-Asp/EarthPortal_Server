using Domain.Entities.Pages;

namespace Domain.Dtos.Pages
{
    public class EnglishPageDto
    {
        public Guid Id { get; set; }

        public EnglishHeader Header { get; set; }

        public EnglishIntro Intro { get; set; }

        public EnglishMainIdea MainIdea { get; set; }

        public EnglishVision Vision { get; set; }

        public EnglishCurrentSituation CurrentSituation { get; set; }

        public List<EnglishItem> Problems { get; set; }
        public List<EnglishItem> Solutions { get; set; }

        public List<EnglishCardContainerDto> BeginningCards { get; set; }
        public List<EnglishCardContainerDto> MiddleCards { get; set; }
        public List<EnglishCardContainerDto> EndCards { get; set; }
    }

    public class EnglishItem
    {
        public Guid Id { get; set; }
        public String Content { get; set; }
    }

    public class EnglishCardContainerDto
    {
        public List<EnglishCardDto> Cards { get; set; }
    }

    public class EnglishCardDto
    {
        public Guid Id { get; set; }

        public String Title { get; set; }
        public String Color { get; set; }
        public String Content { get; set; }
        public String Type { get; set; }
        public bool Line { get; set; }
        public byte Order { get; set; }
    }

}
