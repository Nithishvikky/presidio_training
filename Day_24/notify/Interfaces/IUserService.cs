using Notify.Models.DTOs;
using Notify.Models;

namespace Notify.Interfaces
{
    public interface IUserService
    {
        public Task<User> AddUser(UserAddRequestDto user);
    }
}