using Day14_Task.Interface;
using Day14_Task.Model;
using Day14_Task.Repository;
using Day14_Task.Service;

namespace Day14_Task
{
    public class Program
    {
        static void Main(string[] args)
        {
            UserRepository repo = new UserRepository();
            

            INotifier EmailNotifier = new EmailNotifier();
            INotifier SMSNotifier = new SmsNotifier();

            var userService = new UserService(repo, EmailNotifier); // (DI)UserService depends on INotitfier not on email or sms

            User user = new User { Name = "Alice", Email = "alice@example.com" };
            userService.RegisterUser(user);
        }
    }
}
