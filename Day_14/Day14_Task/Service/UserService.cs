using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day14_Task.Interface;
using Day14_Task.Model;
using Day14_Task.Repository;

namespace Day14_Task.Service
{
    // Only responsible for data logic
    public class UserService
    {
        private readonly UserRepository _repo;
        private readonly INotifier _notifiers; // Depends on INotifier so even we add any other notifier we don't have modify here

        public UserService(UserRepository repo, INotifier notifiers)
        {
            _repo = repo;
            _notifiers = notifiers;
        }

        public void RegisterUser(User user)
        {
            _repo.Add(user);

            _notifiers.Send(user); // Lsp is satisfied — all notifiers behave as expected base class (INotifier)
        }
    }
}
