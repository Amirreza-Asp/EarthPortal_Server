namespace Domain.Entities.Contact
{
    public class Guide
    {
        public Guide(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public String Title { get; set; }
        public String Description { get; set; }
    }
}
