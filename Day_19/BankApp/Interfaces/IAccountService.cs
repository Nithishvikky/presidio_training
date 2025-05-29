using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Interfaces
{
    public interface IAccountService
    {
        public Task<Account> CreateAccount(AccountAddRequestDto account);
        public Task<Account> GetAccountByNumber(string AccountNumber);
    }
}