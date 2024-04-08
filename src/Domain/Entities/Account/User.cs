namespace Domain.Entities.Account
{
    public class User
    {
        public User(Guid roleId, string name, string family, string userName, string password, string? email, string? phoneNumber)
        {
            RoleId = roleId;
            CreatedAt = DateTime.Now;
            Name = name;
            Family = family;
            UserName = userName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public User() { }

        public String Name { get; set; }
        public String Family { get; set; }
        public bool IsActive { get; set; } = true;
        public String UserName { get; set; }
        public String Password { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EnableContentEdit { get; set; } = true;


        public Role? Role { get; set; }
    }
}
