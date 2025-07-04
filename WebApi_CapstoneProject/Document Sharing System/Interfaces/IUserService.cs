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
        public Task<PagedResultDto<User>> GetAllUsers(
            string? searchByEmail = null,
            string? searchByUsername = null,
            string? filterBy = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10
        );
        public Task<User> UpdateUserPassword(Guid Id, ChangePasswordDto passwordDto);

    }
}