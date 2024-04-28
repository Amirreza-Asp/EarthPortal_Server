using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Pages
{
    public class EnglishPage
    {
        public Guid Id { get; set; }

        public EnglishIntro Intro { get; set; }

        public EnglishMainIdea MainIdea { get; set; }

        public EnglishCurrentSituation CurrentSituation { get; set; }

        public EnglishVision Vision { get; set; }

        public List<EnglishCard> Cards { get; set; }

        public ICollection<EnglishProblem> Problems { get; set; }

        public ICollection<EnglishSolution> Solutions { get; set; }
    }

    public class EnglishIntro
    {
        public String Title { get; set; }
        public String Content { get; set; }
    }

    public class EnglishMainIdea
    {
        public String Title { get; set; }
        public String Content1 { get; set; }
        public String Bold { get; set; }
        public String Content2 { get; set; }
    }

    public class EnglishProblem
    {
        public EnglishProblem(string content, Guid englishPageId)
        {
            Id = Guid.NewGuid();
            Content = content;
            EnglishPageId = englishPageId;
        }

        private EnglishProblem()
        {
        }

        public Guid Id { get; set; }
        public String Content { get; set; }

        public Guid EnglishPageId { get; set; }
        [ForeignKey(nameof(EnglishPageId))]
        public EnglishPage? EnglishPage { get; set; }
    }

    public class EnglishSolution
    {
        public EnglishSolution(string content, Guid englishPageId)
        {
            Id = Guid.NewGuid();
            Content = content;
            EnglishPageId = englishPageId;
        }

        private EnglishSolution()
        {
        }

        public Guid Id { get; set; }
        public String Content { get; set; }

        public Guid EnglishPageId { get; set; }
        [ForeignKey(nameof(EnglishPageId))]
        public EnglishPage? EnglishPage { get; set; }
    }

    public class EnglishCurrentSituation
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public String Image { get; set; }
    }

    public class EnglishVision
    {
        public String Title { get; set; }
        public String Content { get; set; }
    }

    public class EnglishCard
    {
        public Guid Id { get; set; }

        public String Title { get; set; }
        public EnglishCardColor Color { get; set; }
        public String Content { get; set; }
        public EnglishCardType Type { get; set; }
        public bool Line { get; set; } = false;
        public byte Order { get; set; } = 1;

        public Guid? SiblingId { get; set; }

        public Guid EnglishPageId { get; set; }
        public EnglishPage? EnglishPage { get; set; }
    }

    public enum EnglishCardType
    {
        Beginning = 10,
        Middle = 20,
        End = 30
    }

    public enum EnglishCardColor
    {
        Yellow = 0,
        Red = 1,
        Green = 2,
        Blue = 3
    }

}
