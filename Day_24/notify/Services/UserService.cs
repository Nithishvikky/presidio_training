using Notify.Models.DTOs;
using Notify.Interfaces;
using Notify.Models;
using Notify.Misc;

namespace Notify.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;

        public UserService(IRepository<string, User> userRepository,
                            IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<User> AddUser(UserAddRequestDto user)
        {
            try
            {
                var MappedUser = new UserMapper().MapUser(user);
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = user.Password
                });
                MappedUser.PasswordHash = encryptedData.EncryptedData;
                MappedUser.HashKey = encryptedData.HashKey;

                var AddedUser = await _userRepository.Add(MappedUser);
                return AddedUser;
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong...{e.Message}");
            }
        }
    }
}