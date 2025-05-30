using Bank.Interfaces;
using Bank.Misc;
using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int, Account> _accountRepository;
        public UserService(IRepository<int, User> userRepository,
                            IRepository<int, Account> accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }
        public async Task<User> RegisterUser(UserAddRequestDto user)
        {
            var AddUser = new UserMapper().MapUser(user);
            return (await _userRepository.Add(AddUser));
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var users = await _userRepository.GetAll();
            var u = users.SingleOrDefault(u => u.Email.Equals(email));
            if (u != null)
            {
                return u;
            }
            throw new Exception($"No User found with {email}");
        }

        public async Task<ICollection<User>> GetUserByName(string name)
        {
            var users = await _userRepository.GetAll();
            var MatchedUsers = users.Where(u => u.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));

            return MatchedUsers.ToList();
        }

        public async Task<ICollection<Account>> GetAccounts(int UserId)
        {
            var Allaccounts = await _accountRepository.GetAll();
            var MatchAccounts = Allaccounts.Where(a => a.UserId == UserId);

            return MatchAccounts.ToList();
        }
    }
}