using Day13_App.Interfaces;
using Day13_App.Models;
using Day13_App.Repositories;
using Day13_App.Services;

namespace Day13_App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IRepositor<int, Employee> employeeRepository = new EmployeeRepository();
            IEmployeeService employeeService = new EmployeeService(employeeRepository); // Dependency Injection
            ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            manageEmployee.Start();
        }
    }
}
