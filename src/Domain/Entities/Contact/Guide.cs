using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class Guide : BaseEntity
    {
        public Guide(string title, string content, bool isPort) : base(Guid.NewGuid())
        {
            Title = title;
            Content = content;
            IsPort = isPort;
        }

        private Guide() { }

        public String Title { get; set; }
        public String Content { get; set; }
        // true is 'درگاه' flase is 'پایش'
        public bool IsPort { get; set; }
    }
}
