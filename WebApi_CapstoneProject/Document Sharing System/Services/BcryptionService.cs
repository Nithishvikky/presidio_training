using DSS.Interfaces;
using BCrypt.Net;
using System.Text;

namespace DSS.Services
{
    public class BcryptionService : IBcryptionService
    {
        public string HashPassword(string password)
        {
            var hasedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hasedPassword;
        }

        public bool VerifyPassword(string password, string hasedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hasedPassword);
        }
    }
}