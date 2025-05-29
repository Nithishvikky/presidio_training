using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUser(UserAddRequestDto user);
        public Task<User> GetUserByEmail(string email);
        public Task<ICollection<User>> GetUserByName(string name);
    }
}