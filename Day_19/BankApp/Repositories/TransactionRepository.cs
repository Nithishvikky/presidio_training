using Bank.Contexts;
using Bank.Models;
using Microsoft.EntityFrameworkCore;


namespace Bank.Repositories
{
    public class TransactionRepository : Repository<int,Transaction>
    {
        public TransactionRepository(BankContext bankContext) : base(bankContext) {}
        public async override Task<Transaction?> Get(int key)
        {
            var transaction = await _bankContext.Transactions.SingleOrDefaultAsync(a => a.Id == key);
            return transaction;
        }

        public async override Task<IEnumerable<Transaction>> GetAll()
        {
            var transactions = _bankContext.Transactions;
            if (transactions.Count() == 0)
                throw new Exception("No Accounts in the database");
            return (await transactions.ToListAsync());
        }
    }
}