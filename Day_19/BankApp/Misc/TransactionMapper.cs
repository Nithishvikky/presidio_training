using Bank.Models;
using Bank.Models.DTOs;

namespace Bank.Misc
{
    public class TransactionMapper
    {
        public Transaction MapTransaction(TransactionAddRequestDto addRequestDto,Account account)
        {
            Transaction transaction = new Transaction();
            transaction.Amount = addRequestDto.Amount;
            transaction.Timestamp = DateTime.UtcNow;
            transaction.Type = addRequestDto.Type;
            transaction.AccountId = account.Id;
            transaction.Account = account;
            return transaction;
        }
    }
}