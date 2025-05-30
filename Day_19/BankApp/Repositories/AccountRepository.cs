using Bank.Contexts;
using Bank.Models;
using Microsoft.EntityFrameworkCore;


namespace Bank.Repositories
{
    public class AccountRepository : Repository<int, Account>
    {
        public AccountRepository(BankContext bankContext) : base(bankContext) { }
        public async override Task<Account?> Get(int key)
        {
            var account = await _bankContext.Accounts.SingleOrDefaultAsync(a => a.Id == key);
            if (account == null)
            {
                throw new Exception("No account found");
            }
            return account;
        }

        public async override Task<IEnumerable<Account>> GetAll()
        {
            var accounts = _bankContext.Accounts;
            if (accounts.Count() == 0)
                throw new Exception("No Accounts in the database");
            return (await accounts.ToListAsync());
        }

        // public async Task<Account> GetAccountAsync(string AccNumber)
        // {
        //     var account = await _bankContext.Accounts.SingleOrDefault(a => a.AccountNumber.Equals(AccNumber));
        //     if (account != null)
        //     {
        //         return account;
        //     }
        //     throw new Exception($"No account found {AccNumber}");
        // }
    }
}