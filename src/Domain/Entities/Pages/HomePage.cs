namespace Domain.Entities.Pages
{
    public class HomePage
    {
        public HomePage()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public HomeHeader Header { get; set; }
        public HomeWork Work { get; set; }
    }

    public class HomeHeader
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public bool PortBtnEnable { get; set; }
        public bool AppBtnEnable { get; set; }
    }

    public class HomeWork
    {
        public String Title { get; set; }
        public String Content { get; set; }
        public String Port { get; set; }
        public String App { get; set; }
    }
}
