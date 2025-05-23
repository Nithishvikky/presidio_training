using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day15_Task.Interface;
using Day15_Task.Model;
using Day15_Task.Service;

namespace Day15_Task.ProxyDesign_Task
{
    public class ClientCode
    {
        private static string path = "C:\\Users\\DELL\\Desktop\\Work\\Day_15\\Day15_Task\\file.txt";
        public void Start()
        {

            User u = new User();
            u.GetDetailsFromUser();
            Console.WriteLine(u);

            IFile file = new ProxyFile(path, u);
            file.Read();
        } 
    }
}
