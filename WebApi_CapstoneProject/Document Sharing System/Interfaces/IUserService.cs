using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IUserService
    {
        public Task<User> AddUser(UserAddRequestDto user);
        public Task<User> GetUserByEmail(string Email);
        public Task<User> GetUserById(Guid Id);
        public Task<IEnumerable<User>> GetAllUsersOnly();
        public Task<User> UpdateUserPassword(Guid Id, ChangePasswordDto passwordDto);
    }
}