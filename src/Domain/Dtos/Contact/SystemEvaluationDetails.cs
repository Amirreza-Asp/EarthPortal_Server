namespace Domain.Dtos.Contact
{
    public class SystemEvaluationDetails
    {
        public int TotalVote { get; set; }
        public List<UsefulPage> Pages { get; set; }
        public List<IntrodMethod> IntroductionMethods { get; set; }
        public List<Vote> Votes { get; set; }
    }

    public class UsefulPage
    {
        public String Page { get; set; }
        public int Count { get; set; }
    }

    public class IntrodMethod
    {
        public String Method { get; set; }
        public int Count { get; set; }
    }

    public class Vote
    {
        public int Score { get; set; }
        public int Count { get; set; }
    }
}
