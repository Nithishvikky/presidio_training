using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day14_Task.Model;

namespace Day14_Task.Repository
{
    // Only responsible for interact with database/Inmemory
    public class UserRepository
    {
        private List<User> _users = new();

        public void Add(User user)
        {
            _users.Add(user);
        }

        public List<User> GetAll()
        {
            return _users;
        }
    }
}
