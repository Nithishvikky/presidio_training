using DSS.Models;

namespace DSS.Interfaces
{
    public interface IBcryptionService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hasedPassword);
    }
}