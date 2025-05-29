using Bank.Models;
using Bank.Models.DTOs;

namespace Bank.Misc
{
    public class AccountMapper
    {
        public Account MapAccount(AccountAddRequestDto accountRequestDto)
        {
            Account account = new Account();
            account.Balance = accountRequestDto.Balance;
            account.UserId = accountRequestDto.UserId;
            return account;
        }
    }
}