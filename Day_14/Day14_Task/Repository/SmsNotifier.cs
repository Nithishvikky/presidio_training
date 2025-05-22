using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day14_Task.Interface;
using Day14_Task.Model;

namespace Day14_Task.Repository
{
    // Concrete class
    public class SmsNotifier : INotifier
    {  
        public void Send(User user)
        {
            Console.WriteLine($"SMS sent to {user.Name}");
        }
    }
}
