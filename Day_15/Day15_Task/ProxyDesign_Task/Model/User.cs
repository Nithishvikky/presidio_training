using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day15_Task.ProxyDesign_Task.Model
{
    public class User
    {
        public string UserName { get; set; }
        public string Role {  get; set; }

        public User() { }
        public User(string name,string role) 
        {
            UserName = name;
            Role = role.ToLower();
        }
        public void GetDetailsFromUser()
        {
            Console.Write("Please Enter User Name : ");
            UserName = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrEmpty(UserName))
            {
                Console.Write("Please Enter valid user Name : ");
                UserName = Console.ReadLine() ?? string.Empty;
            }
            Console.Write("Please Enter User Role : ");
            Role = Console.ReadLine() ?? string.Empty;

            // Implemented IsValid Role to get proper roles between (Admin/User/Guest)
            while (string.IsNullOrEmpty(Role) || !IsValidRole(Role))
            {
                Console.Write("Please Enter valid user Role : ");
                Role = Console.ReadLine() ?? string.Empty;
            }
            Role = Role.ToLower();
        }
        public override string ToString()
        {
            return "\nUser : " + UserName +" | Role : " + Role;
        }
        public bool IsValidRole(string role)
        {
            role = role.ToLower();

            if (role == "admin" || role == "user" || role == "guest")
            {
                return true;
            }
            return false;
        }
    }
}
