using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Day12
{
    internal class Hard
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
        static bool IsEmployeeExists(List<KeyValuePair<int, Employee>> li)
        {
            return li.Count > 0;
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
        static KeyValuePair<int, Employee> AddEmployee()
        {
            // Don't need to get Employee Id as input from user
            // Auto incremental like in database
            Employee e = new Employee(employeeId);
            TakeEmployeeDetailsFromUser(e);
            employeeId++; // Incrementing inorder to prevent the next employee to be added in existing Employee Id 

            return new KeyValuePair<int, Employee>(e.Id, e);
        }
        static void DisplayAllEmployee(List<KeyValuePair<int, Employee>> li)
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
                Console.WriteLine($"\n-----Employee {e.Key}-----");
                Console.WriteLine(e.Value.ToString()); // ToString() is a overriden function in the Employee class
            }
            else
            {
                Console.WriteLine($"There is No employee");
            }
        }
        static KeyValuePair<int, Employee> FindEmployeeById(List<KeyValuePair<int, Employee>> li, int EmployeeId)
        {
            KeyValuePair<int, Employee> res = li.Where(e => e.Value.Id == EmployeeId).FirstOrDefault();
            return res;
        }
        static void ModifyEmployee(List<KeyValuePair<int, Employee>> li)
        {
            if (!IsEmployeeExists(li))
            {
                DisplayAllEmployee(li); // It will display the exception to the user if no employess exists
                return;
            }

            Console.Write("\nEnter Employee Id to Modify : ");
            int ModifyId = UserInputNumValidation();

            KeyValuePair<int, Employee> e = FindEmployeeById(li, ModifyId);

            DisplayEmployee(e); // To see the employee details for reference
            // It will show whether given id exists or not

            if (e.Value == null) return;

            TakeEmployeeDetailsFromUser(e.Value); // Function from Employee class
        }
        static void DeleteEmployee(List<KeyValuePair<int, Employee>> li)
        {
            if (!IsEmployeeExists(li))
            {
                DisplayAllEmployee(li); // It will display the exception to the user if no employess exists
                return;
            }

            Console.Write("\nEnter Employee Id to Delete : ");
            int DeleteId = UserInputNumValidation();

            KeyValuePair<int, Employee> e = FindEmployeeById(li, DeleteId);

            DisplayEmployee(e); // To see the employee details for reference 
            // It will show whether given id exists or not

            if (e.Value == null) return;

            li.Remove(e); // Remove the kvp from list
        }
        public void Run()
        {
            bool flag = true;
            List<KeyValuePair<int, Employee>> Employees = new List<KeyValuePair<int, Employee>>();

            while (flag)
            {
                Console.WriteLine("\nChoose from Menu\n\t1. Add Employee\n\t2. Display All Employees" +
                    "\n\t3. Display Employee(Id)\n\t4. Modify Employee details\n\t5. Delete Employee\n\t6. Exit");
                Console.Write("\nEnter the option from menu : ");
                int op = UserInputNumValidation();

                switch (op)
                {
                    case 1: Employees.Add(AddEmployee());
                            break;

                    case 2: DisplayAllEmployee(Employees);
                            break;

                    case 3: if (IsEmployeeExists(Employees))
                            {
                                Console.Write("\nEnter Employee Id to display : ");
                                int FindId = UserInputNumValidation();
                                DisplayEmployee(FindEmployeeById(Employees, FindId));
                            }
                            else { DisplayAllEmployee(Employees); } // It will display the exception to the user if no employess exists
                            break;

                    case 4: ModifyEmployee(Employees);
                            break;

                    case 5: DeleteEmployee(Employees);
                            break;

                    case 6: flag = false;
                            break;

                    default: Console.WriteLine("\nEnter valid Option from menu....");
                            break;

                }
            }
        }
    }
}
