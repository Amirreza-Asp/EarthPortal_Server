using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class FrequentlyAskedQuestions : BaseEntity
    {
        public FrequentlyAskedQuestions(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public String Title { get; set; }
        public String Content { get; set; }
    }
}
