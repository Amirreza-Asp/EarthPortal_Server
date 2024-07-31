using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class FrequentlyAskedQuestions : BaseEntity
    {
        public FrequentlyAskedQuestions(string title, string content) : base(Guid.NewGuid())
        {
            Title = title;
            Content = content;
        }

        private FrequentlyAskedQuestions() { }

        public String Title { get; set; }
        public String Content { get; set; }
    }
}
