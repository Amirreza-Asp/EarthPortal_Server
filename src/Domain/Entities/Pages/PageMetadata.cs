using Domain.Shared;

namespace Domain.Entities.Pages
{
    public class PageMetadata : BaseEntity
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public Page Page { get; set; }
        public ICollection<PageMetadataKeywords> Keywords { get; set; }


        private PageMetadata()
        {
        }

        public PageMetadata(string title, string description, Page page, List<string> keywords) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Page = page;
            Keywords = keywords.Select(keyword => new PageMetadataKeywords(keyword)).ToList();
        }
    }

    public class PageMetadataKeywords
    {
        public String Value { get; set; }

        public PageMetadataKeywords(string value)
        {
            Value = value;
        }

        private PageMetadataKeywords()
        {
        }
    }

    public enum Page
    {
        Home = 0,
        News = 10,
        Law = 20,
        Multimedia = 30,
        DeviceEvaluation = 40,
        AboutUs = 50,
        ContactWithUs = 60,
        EducationVideos = 70,
        GeneralEvaluationForm = 80
    }
}
