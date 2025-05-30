using Bank.Models;
using Bank.Models.DTOs;


namespace Bank.Interfaces
{
    public interface ITransactionService
    {
        public Task<Transaction> MadeTransaction(TransactionAddRequestDto transaction);

        public Task<ICollection<Transaction>> GetTransactionsByAccountNumber(int AccId);
    }
}