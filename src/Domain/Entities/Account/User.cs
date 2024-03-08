namespace Domain.Entities.Account
{
    public class User
    {
        public User(string nationalCode, Guid roleId, string name, string family, string userName, string password, string email, string phoneNumber)
        {
            NationalCode = nationalCode;
            RoleId = roleId;
            CreatedAt = DateTime.Now;
            Name = name;
            Family = family;
            UserName = userName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        private User() { }

        public String NationalCode { get; set; }
        public String Name { get; set; }
        public String Family { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }


        public Role? Role { get; set; }
    }
}
