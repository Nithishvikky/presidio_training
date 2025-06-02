using Bank.Models;
using Bank.Models.DTOs;

namespace Bank.Misc
{
    public class UserMapper
    {
        public User MapUser(UserAddRequestDto addRequestDto)
        {
            User user = new User();
            user.FullName = addRequestDto.FullName;
            user.Email = addRequestDto.Email;
            return user;
        }
    }
}