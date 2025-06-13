using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Misc
{
    public class UserMapper
    {
        public User MapUser(UserAddRequestDto userAddRequestDto)
        {
            User user = new User();
            
            user.Username = InputSanitizer.Sanitize(userAddRequestDto.Username);
            user.Email = InputSanitizer.Sanitize(userAddRequestDto.Email);
            user.Role = userAddRequestDto.Role;

            return user;
        }
    }
}