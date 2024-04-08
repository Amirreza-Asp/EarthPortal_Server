using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pages
{
    public class AboutUsPage
    {

        public AboutUsPage(string title, string content, string footer)
        {
            Id = Guid.NewGuid();
            Title = title;
            Content = content;
            Footer = footer;
        }

        [Key]
        public Guid Id { get; set; }

        public String Title { get; set; }
        public String Content { get; set; }
        public String Footer { get; set; }
    }
}
