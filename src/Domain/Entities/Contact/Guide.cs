using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Guide : BaseEntity
    {
        public Guide(string title, string content, bool isPort)
        {
            Title = title;
            Content = content;
            IsPort = isPort;
        }

        public String Title { get; set; }
        public String Content { get; set; }
        public bool IsPort { get; set; }
    }
}
