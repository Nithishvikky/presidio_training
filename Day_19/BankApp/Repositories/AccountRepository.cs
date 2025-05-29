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
            return account;
        }

        public async override Task<IEnumerable<Account>> GetAll()
        {
            var accounts = _bankContext.Accounts;
            if (accounts.Count() == 0)
                throw new Exception("No Accounts in the database");
            return (await accounts.ToListAsync());
        }
    }
}