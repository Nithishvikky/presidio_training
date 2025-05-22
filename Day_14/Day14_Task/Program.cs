using Day14_Task.Interface;
using Day14_Task.Model;
using Day14_Task.Repository;

namespace Day14_Task
{
    public class Program
    {
        static void Main(string[] args)
        {
            UserRepository repo = new UserRepository();
            //var notifiers = new List<INotifier> {
            //    new EmailNotifier(),
            //    new SmsNotifier()
            //};

            INotifier EmailNotifier = new EmailNotifier();
            INotifier SMSNotifier = new SmsNotifier();

            var userService = new UserService(repo, SMSNotifier); // (DI)UserService depends on INotitfier not on email or sms

            User user = new User { Name = "Alice", Email = "alice@example.com" };
            userService.RegisterUser(user);
        }
    }
}
