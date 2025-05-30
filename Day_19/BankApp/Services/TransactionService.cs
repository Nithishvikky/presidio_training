using Bank.Contexts;
using Bank.Interfaces;
using Bank.Misc;
using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<int, Account> _accountRepository;
        private readonly IRepository<int, Transaction> _transactionRepository;

        private readonly BankContext _bankContext;
        public TransactionService(IRepository<int, Transaction> transactionRepository,
                                IRepository<int, Account> accountRepository,
                                BankContext bankContext)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _bankContext = bankContext;
        }
        public async Task<Transaction> MadeTransaction(TransactionAddRequestDto transaction)
        {
            using var sqlTransaction = await _bankContext.Database.BeginTransactionAsync();
            try
            {
                var account = await GetAccoundId(transaction.AccountNumber);

                var AddedTransaction = new TransactionMapper().MapTransaction(transaction,account);

                await _bankContext.Transactions.AddAsync(AddedTransaction);
                await _bankContext.SaveChangesAsync();

                if (!BalanceChecker(AddedTransaction, account.Balance))
                {
                    throw new Exception("Insufficient balance");
                }

                if (AddedTransaction.Type.Equals("Withdraw"))
                {
                    account.Balance -= AddedTransaction.Amount;
                }
                else
                {
                    account.Balance += AddedTransaction.Amount;
                }

                await _accountRepository.Update(account.Id, account);

                AddedTransaction.Account = account;
                await _transactionRepository.Update(AddedTransaction.Id, AddedTransaction);

                await _bankContext.SaveChangesAsync();
                await sqlTransaction.CommitAsync();

                return AddedTransaction;

            }
            catch (Exception e)
            {
                await sqlTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Account> GetAccoundId(string AccNumber)
        {
            var accounts = await _accountRepository.GetAll();
            var account = accounts.SingleOrDefault(a => a.AccountNumber.Equals(AccNumber));
            if (account != null)
            {
                return account;
            }
            throw new Exception("No account found");
        }

        public bool BalanceChecker(Transaction transaction, decimal balance)
        {
            if (transaction.Type.Equals("Withdraw"))
            {
                if (balance < transaction.Amount)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<ICollection<Transaction>> GetTransactionsByAccountNumber(int id)
        {
            var transactions = await _transactionRepository.GetAll();
            var MatchedTransactions = transactions.Where(u => u.AccountId == id);

            return MatchedTransactions.ToList();
        }
    }
}