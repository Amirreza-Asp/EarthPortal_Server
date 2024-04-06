namespace Domain.Dtos.Account
{
    public class UserSummary
    {
        public String UserName { get; set; }
        public String Name { get; set; }
        public String Family { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public String Role { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
