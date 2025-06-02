using Bank.Interfaces;
using Bank.Misc;
using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<int, Account> _accountRepository;
        private readonly IRepository<int, User> _userRepository;
        public AccountService(IRepository<int, Account> accountRepository,
                              IRepository<int, User> userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }
        public async Task<Account> CreateAccount(AccountAddRequestDto account)
        {
            var AddedAccount = new AccountMapper().MapAccount(account);
            AddedAccount = await _accountRepository.Add(AddedAccount); // bal,userid

            AddedAccount.AccountNumber = GenerateAccountNumber(AddedAccount.Id);
            AddedAccount.User =await _userRepository.Get(AddedAccount.UserId);
            
            AddedAccount = await _accountRepository.Update(AddedAccount.Id, AddedAccount);
        

            return AddedAccount;
        }

        public string GenerateAccountNumber(int Id)
        {
            return $"ACC{Id.ToString().PadLeft(13, '0')}";
        }

        public async Task<Account> GetAccountByNumber(string AccountNumber)
        {
            var accounts = await _accountRepository.GetAll();
            var account = accounts.SingleOrDefault(a => a.AccountNumber.Equals(AccountNumber));
            if (account != null)
            {
                return account;
            }
            throw new Exception($"No account found {AccountNumber}");
        }
    }
}