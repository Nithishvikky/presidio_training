using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Misc;
using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IBcryptionService _bcryptionService;
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<Guid, User> userRepository,
                           IBcryptionService bcryptionService,
                           ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _bcryptionService = bcryptionService;
            _logger = logger;
        }

        public async Task<User> AddUser(UserAddRequestDto user)
        {
            try
            {
                _logger.LogInformation("Attempting to add new user with email: {Email}", user.Email);

                var userObject = new UserMapper().MapUser(user);
                if (userObject == null)
                {
                    _logger.LogWarning("User mapping returned null for email: {Email}", user.Email);
                    throw new InvalidOperationException("User details are empty");
                }

                var users = await _userRepository.GetAll();
                if (users.Any(u => u.Email.Equals(userObject.Email)))
                {
                    _logger.LogWarning("User with email {Email} already exists", user.Email);
                    throw new InvalidOperationException("Email already Exists");
                }

                userObject.PasswordHash = _bcryptionService.HashPassword(user.Password);
                var result = await _userRepository.Add(userObject);

                _logger.LogInformation("User with email {Email} successfully added", user.Email);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user with email: {Email}", user.Email);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersOnly()
        {
            try
            {
                var allUsers = await _userRepository.GetAll();
                if (!allUsers.Any())
                {
                    _logger.LogWarning("No one registered yet");
                    throw new ArgumentNullException("No one registered yet");
                }

                var userOnly = allUsers.Where(u => u.Role.Equals("User"));
                if (!userOnly.Any())
                {
                    _logger.LogWarning("No one registered as user");
                    throw new ArgumentNullException("No one registered as user");
                }
                return userOnly;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all users");
                throw;
            }
        }

        public async Task<User> GetUserByEmail(string Email)
        {
            try
            {
                _logger.LogInformation("Fetching user by email: {Email}", Email);
                var users = await _userRepository.GetAll();
                var user = users.SingleOrDefault(u => u.Email.Equals(Email));

                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", Email);
                    throw new KeyNotFoundException("User not found");
                }

                _logger.LogInformation("User found for email: {Email}", Email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with email: {Email}", Email);
                throw;
            }
        }

        public async Task<User> GetUserById(Guid Id)
        {
            try
            {
                _logger.LogInformation("Fetching user by ID: {UserId}", Id);
                return await _userRepository.Get(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", Id);
                throw;
            }
        }

        public async Task<User> UpdateUserPassword(Guid Id, ChangePasswordDto passwordDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update password for user ID: {UserId}", Id);

                var user = await _userRepository.Get(Id);
                if (_bcryptionService.VerifyPassword(passwordDto.OldPassword, user.PasswordHash))
                {
                    user.PasswordHash = _bcryptionService.HashPassword(passwordDto.NewPassword);
                    user.UpdatedAt = DateTime.UtcNow;

                    var updatedUser = await _userRepository.Update(Id, user);

                    _logger.LogInformation("Password updated successfully for user ID: {UserId}", Id);
                    return updatedUser;
                }

                _logger.LogWarning("Password verification failed for user ID: {UserId}", Id);
                throw new InvalidOperationException("Password invalid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user ID: {UserId}", Id);
                throw;
            }
        }
    }
}