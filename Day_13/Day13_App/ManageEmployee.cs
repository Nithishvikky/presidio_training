
using Day13_App.Interfaces;
using Day13_App.Models;


namespace Day13_App
{
    public class ManageEmployee
    {
        private readonly IEmployeeService _employeeService;

        public ManageEmployee(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                int option = 0;
                while (!int.TryParse(Console.ReadLine(), out option) || (option < 1 && option > 2))
                {
                    Console.WriteLine("Invalid entry. Please enter a valid option");
                }
                switch (option)
                {
                    case 1:
                        AddEmployee();
                        break;
                    case 2:
                        SearchEmployee();
                        break;
                    default:
                        exit = true;
                        break;
                }
            }
        }
        public void PrintMenu()
        {
            Console.WriteLine("Choose what you wanted");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Search Employee");
        }
        public void AddEmployee()
        {
            Employee employee = new Employee();
            employee.TakeEmployeeDetailsFromUser();
            int id = _employeeService.AddEmployee(employee);
            Console.WriteLine($"The employee added. The Id is {id}");
        }
        public void SearchEmployee()
        {
            var searchMenu = PrintSearchMenu();
            var employees = _employeeService.SearchEmployee(searchMenu);
            Console.WriteLine("The search options you have selected");
            Console.WriteLine(searchMenu);
            if ((employees == null))
            {
                Console.WriteLine("No Employees for the search");
            }
            PrintEmployees(employees);

        }

        private void PrintEmployees(List<Employee>? employees)
        {
            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }
        }

        private SearchModel PrintSearchMenu()
        {
            Console.WriteLine("Please select the search option");
            SearchModel searchModel = new SearchModel();
            Console.WriteLine("Search by ID? Type 1 for yes Type 2 no");
            int idOption = 0;
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if (idOption == 1)
            {
                Console.WriteLine("Please enter the employee ID");
                int id;
                while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
                {
                    Console.WriteLine("Invalid entry for ID. Please enter a valid employee ID");
                }
                searchModel.Id = id;
                idOption = 0;
                return searchModel;
            }
            Console.WriteLine("Search by Name. ? Type 1 for yes Type 2 no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if (idOption == 1)
            {
                Console.WriteLine("Please enter the employee Name");
                string name = Console.ReadLine() ?? "";
                searchModel.Name = name;
                idOption = 0;
            }
            Console.WriteLine("3. Search by Age. Please enter 1 for yes and 2 for no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if (idOption == 1)
            {
                searchModel.Age = new Range<int>();
                int age;
                Console.WriteLine("Please enter the min employee Age");
                while (!int.TryParse(Console.ReadLine(), out age) || age <= 18)
                {
                    Console.WriteLine("Invalid entry for min age. Please enter a valid employee age");
                }
                searchModel.Age.MinVal = age;
                Console.WriteLine("Please enter the max employee Age");
                while (!int.TryParse(Console.ReadLine(), out age) || age <= 18)
                {
                    Console.WriteLine("Invalid entry for max age. Please enter a valid employee age");
                }
                searchModel.Age.MaxVal = age;
                idOption = 0;
            }

            Console.WriteLine("4. Search by Salary. Please enter 1 for yes and 2 for no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if (idOption == 1)
            {
                searchModel.Salary = new Range<double>();
                double salary;
                Console.WriteLine("Please enter the min employee Salary");
                while (!double.TryParse(Console.ReadLine(), out salary) || salary <= 0)
                {
                    Console.WriteLine("Invalid entry for min salary. Please enter a valid employee salary");
                }
                searchModel.Salary.MinVal = salary;
                Console.WriteLine("Please enter the max employee Salary");
                while (!double.TryParse(Console.ReadLine(), out salary) || salary <= 0)
                {
                    Console.WriteLine("Invalid entry for max salary. Please enter a valid employee salary");
                }
                searchModel.Salary.MaxVal = salary;
                idOption = 0;
            }
            return searchModel;
        }
    }
}
