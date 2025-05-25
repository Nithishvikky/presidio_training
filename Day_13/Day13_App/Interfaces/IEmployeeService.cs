using Day13_App.Models;

namespace Day13_App.Interfaces
{
    public interface IEmployeeService
    {
        int AddEmployee(Employee employee);
        List<Employee>? SearchEmployee(SearchModel searchModel);
    }
}
