using Day13_App.Interfaces;
using Day13_App.Models;
using Day13_App.Repositories;
using Day13_App.Services;
using Day13_App.Misc;

namespace Day13_App
{
    internal class Program
    {
        List<Employee> employees = new List<Employee>()
        {
            new Employee(101,30, "John Doe",  50000),
            new Employee(102, 25,"Jane Smith",  60000),
            new Employee(103,35, "Sam Brown",  70000)
        };
        //public delegate void MyDelegate<T>(T num1, T num2);
        //public delegate void MyFDelegate(float num1, float num2);
        public void Add(int n1, int n2)
        {
            int sum = n1 + n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
        }
        public void Product(int n1, int n2)
        {
            int prod = n1 * n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {prod}");
        }
        Program()
        {
            //MyDelegate<int> del = new MyDelegate<int>(Add);
            Action<int, int> del = Add;
            del += Product;
            //del += delegate (int n1, int n2)
            //{
            //    Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            //};
            del += (int n1, int n2) => Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            del(100, 20);
        }
        void FindEmployee()
        {
            int empId = 102;
            Predicate<Employee> predicate = e => e.Id == empId;
            Employee? emp = employees.Find(predicate);
            Console.WriteLine(emp?.ToString() ?? "No such employee");
        }
        void SortEmployee()
        {
            var sortedEmployees = employees.OrderBy(e => e.Name);
            foreach (var emp in sortedEmployees)
            {
                Console.WriteLine(emp.ToString());
            }
        }
        void FilterEmployee()
        {
            var FilteredEmployees = employees.Where(e => e.Name == "nithish");
        }
        static void Main(string[] args)
        {
            //IRepositor<int, Employee> employeeRepository = new EmployeeRepository();
            //IEmployeeService employeeService = new EmployeeService(employeeRepository);
            //ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            //manageEmployee.Start();
            //new Program();
            //Program program = new();
            //program.FindEmployee();
            //program.SortEmployee();
            string str1 = "Stud";
            Console.WriteLine(str1.StringValidationCheck(5));

        }
    }
}
