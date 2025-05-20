using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Day12
{
    internal class Medium 
    {
        static int employeeId = 1; // To ensure the sequence of employee Id and unique
        static string UserInputValidation()
        {
            string? input;
            while (String.IsNullOrEmpty(input = Console.ReadLine()?.Trim()))
            {
                Console.Write("Enter valid word : ");
            }

            return input;
        }
        static int UserInputNumValidation()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Enter valid number : ");
            }
            return num;
        }
        static void TakeEmployeeDetailsFromUser(Employee e)
        {
            Console.Write("\nPlease enter the employee name : ");
            e.Name = UserInputValidation();

            Console.Write("Please enter the employee age : ");
            e.Age = UserInputNumValidation();

            Console.Write("Please enter the employee salary : ");
            e.Salary = UserInputNumValidation();
        }
        static KeyValuePair<int,Employee> AddEmployee()
        {
            Employee e = new Employee(employeeId);
            TakeEmployeeDetailsFromUser(e);
            employeeId++; // Incrementing inorder to prevent the next employee to be added in existing Employee Id 

            return new KeyValuePair<int, Employee>(e.Id, e);
        }
        static void DisplayAllEmployee(List<KeyValuePair<int,Employee>> li)
        {
            if (li.Count > 0)
            {
                foreach (KeyValuePair<int, Employee> e in li)
                {
                    DisplayEmployee(e);
                }
            }
            else
            {
                Console.WriteLine($"There is no employee in the list");
            }
            
        }
        static void DisplayEmployee(KeyValuePair<int, Employee> e)
        {
            if (e.Value != null)
            {
                Console.WriteLine($"-----Employee {e.Key}-----");
                Console.WriteLine(e.Value.ToString()); // ToString() is a overriden function in the Employee class
            }
            else
            {
                Console.WriteLine($"No employee");
            }
        }
        static List<KeyValuePair<int, Employee>> SortEmployeesBySalary(List<KeyValuePair<int, Employee>> li)
        {
            li.Sort((a,b) => a.Value.Salary.CompareTo(b.Value.Salary));
            return li;
        }
        static KeyValuePair<int, Employee> FindEmployeeById(List<KeyValuePair<int, Employee>> li,int EmployeeId)
        {
            KeyValuePair<int, Employee> res = li.Where(e => e.Value.Id == EmployeeId).FirstOrDefault();
            return res;   
        }
        static List<KeyValuePair<int, Employee>> FindAllEmployeesByName(List<KeyValuePair<int, Employee>> li, string Name)
        {
            List<KeyValuePair<int, Employee>> res = li.Where(e => e.Value.Name.Equals(Name)).ToList();
            return res;
        }
        static List<KeyValuePair<int, Employee>> FindAllEmployeeByAge(List<KeyValuePair<int, Employee>> li, int EmployeeId)
        {
            KeyValuePair<int, Employee> e = FindEmployeeById(li, EmployeeId);

            if (e.Value == null) 
            { 
                return null; 
            }
            int EmployeeAge = e.Value.Age;

            List<KeyValuePair<int, Employee>> res = li.Where(e => e.Value.Age > EmployeeAge).ToList();
            return res;
        }
        public void Run()
        {
            List<KeyValuePair<int,Employee>> li = new List<KeyValuePair<int, Employee>>();

            Console.Write("Enter the Number of employees you want to add : ");
            int n = UserInputNumValidation();

            for(int i = 0; i < n; i++)
            {
                KeyValuePair<int, Employee> e = AddEmployee();
                li.Add(e);
            }

            DisplayAllEmployee(SortEmployeesBySalary(li)); // Return list of kvp

            Console.Write("Enter the employee Id to find : ");
            int searchId = UserInputNumValidation();
            DisplayEmployee(FindEmployeeById(li, searchId)); // Return kvp

            Console.Write("Enter the employee Name to find : ");
            string name = UserInputValidation();
            DisplayAllEmployee(FindAllEmployeesByName(li, name)); // Return list of kvp

            Console.Write("Enter the employee Id to find who are older : ");
            int searchAgeId = UserInputNumValidation();
            DisplayAllEmployee(FindAllEmployeeByAge(li, searchAgeId)); // Return list of kvp

        }
    }
}
