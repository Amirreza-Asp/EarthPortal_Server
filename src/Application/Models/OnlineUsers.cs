namespace Application.Models
{
    public class OnlineUserData
    {
        public OnlineUserData(String id)
        {
            Id = id;
            ValidTime = DateTime.Now.AddMinutes(5);
        }

        public String Id { get; set; }
        public DateTime ValidTime { get; set; }

        public bool IsValid => ValidTime > DateTime.Now;
    }
}
