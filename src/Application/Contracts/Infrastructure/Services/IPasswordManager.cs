namespace Application.Contracts.Infrastructure.Services
{
    public interface IPasswordManager
    {
        public string HashPassword(string password);

        public bool VerifyPassword(string hashedPassword, string enteredPassword);
    }
}
