using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day13_App.Interfaces;
using Day13_App.Models;

namespace Day13_App.Services
{
    public class EmployeeService : IEmployeeService
    {
        IRepositor<int, Employee> _employeeRepository;
        public EmployeeService(IRepositor<int, Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public int AddEmployee(Employee employee)
        {
            try
            {
                var result = _employeeRepository.Add(employee);
                if (result != null)
                {
                    return result.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        public List<Employee>? SearchEmployee(SearchModel searchModel)
        {
            try
            {
                var employees = _employeeRepository.GetAll();
                employees = SearchById(employees, searchModel.Id);
                employees = SearchByName(employees, searchModel.Name);
                employees = SeachByAge(employees, searchModel.Age);
                employees = SearchBySalary(employees, searchModel.Salary);
                if (employees != null && employees.Count > 0)
                    return employees.ToList(); ;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private ICollection<Employee> SearchBySalary(ICollection<Employee> employees, Range<double>? salary)
        {
            if (salary == null || employees.Count == 0 || employees == null)
            {
                return employees;
            }
            else
            {
                return employees.Where(e => e.Salary >= salary.MinVal && e.Salary <= salary.MaxVal).ToList();
            }
        }

        private ICollection<Employee> SeachByAge(ICollection<Employee> employees, Range<int>? age)
        {
            if (age == null || employees.Count == 0 || employees == null)
            {
                return employees;
            }
            else
            {
                return employees.Where(e => e.Age >= age.MinVal && e.Age <= age.MaxVal).ToList();
            }
        }

        private ICollection<Employee> SearchByName(ICollection<Employee> employees, string? name)
        {
            if (name == null || employees == null || employees.Count == 0)
            {
                return employees;
            }
            else
            {
                return employees.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            }
        }

        private ICollection<Employee> SearchById(ICollection<Employee> employees, int? id)
        {
            if (id == null || employees == null || employees.Count == 0)
            {
                return employees;
            }
            else
            {
                return employees.Where(e => e.Id == id).ToList();
            }
        }
    }
}
