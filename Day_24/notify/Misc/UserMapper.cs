using Notify.Models.DTOs;
using Notify.Models;

namespace Notify.Misc
{
    public class UserMapper
    {
        public User MapUser(UserAddRequestDto addRequestDto)
        {
            User user = new User();
            user.Username = addRequestDto.Email;
            user.Role = addRequestDto.Role;

            return user;
        }
    }
}