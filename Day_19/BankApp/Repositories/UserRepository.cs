using Bank.Contexts;
using Bank.Models;
using Microsoft.EntityFrameworkCore;


namespace Bank.Repositories
{
    public class UserRepository : Repository<int, User>
    {
        public UserRepository(BankContext bankContext) : base(bankContext) {}
        public async override Task<User?> Get(int key)
        {
            var user = await _bankContext.Users.SingleOrDefaultAsync(a => a.Id == key);
            return user;
        }
    
        public async override Task<IEnumerable<User>> GetAll()
        {
            var users = _bankContext.Users;
            if (users.Count() == 0)
                throw new Exception("No Users in the database");
            return (await users.ToListAsync());
        }
    }
}