using Day14_Task.Interface;
using Day14_Task.Model;

namespace Day14_Task.Repository
{
    // Concrete class
    public class EmailNotifier : INotifier, IOnlineNotifier
    {
        public void NameAloneForEmail(User user)
        {
            Console.WriteLine($"Email sent to {user.Name} : {user.Email}");
        }

        public void Send(User user)
        {
            Console.WriteLine($"Email sent to {user.Email}");
        }
    }
}
