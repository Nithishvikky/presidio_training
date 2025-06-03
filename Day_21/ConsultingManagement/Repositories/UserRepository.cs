using ConsultingManagement.Contexts;
using ConsultingManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(ConsultancyContext context) : base(context) { }
        
        public override async Task<User> Get(string key)
        {
            return await _consultancyContext.users.SingleOrDefaultAsync(u => u.Username == key);
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _consultancyContext.users.ToListAsync();
        }
           
    }
}