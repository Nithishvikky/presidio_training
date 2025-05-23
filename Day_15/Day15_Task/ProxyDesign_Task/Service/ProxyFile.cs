using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day15_Task.ProxyDesign_Task.Interface;
using Day15_Task.ProxyDesign_Task.Model;

namespace Day15_Task.ProxyDesign_Task.Service
{
    public class ProxyFile : IFile
    {
        private readonly File _realFile;
        private readonly User _user;

        public ProxyFile(string filePath, User user)
        {
            _realFile = new File(filePath);
            _user = user;
        }

        public void Read()
        {             
            if (CheckAccess())
            {
                if (_user.Role == "admin")
                {
                    _realFile.Read(); 
                }
                else 
                {
                    _realFile.MetaData();
                }
            }
        }

        public bool CheckAccess()
        {
            if (_user.Role == "admin" || _user.Role == "user")
            {
                Console.WriteLine("\n[Access Granted]");
                return true;
            }
            Console.WriteLine("\n[Access Denied]");
            return false;
        }
    }
}
