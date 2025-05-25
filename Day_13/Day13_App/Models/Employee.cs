

namespace Day13_App.Models
{
    public class Employee : IComparable<Employee>, IEquatable<Employee>
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }

        public Employee()
        {
            Name = string.Empty;
        }

        public Employee(int id, int age, string name, double salary)
        {
            Id = id;
            Age = age;
            Name = name;
            Salary = salary;
        }

        public void TakeEmployeeDetailsFromUser()
        {
            Console.WriteLine("Please enter the employee ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("Invalid entry for ID. Please enter a valid employee ID");
            }
            Id = id;
            Console.WriteLine("Please enter the employee name");
            Name = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the employee age");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age <= 18)
            {
                Console.WriteLine("Invalid entry for age. Please enter a valid employee age");
            }
            Age = age;
            Console.WriteLine("Please enter the employee salary");
            float salary;
            while (!float.TryParse(Console.ReadLine(), out salary) || salary <= 0)
            {
                Console.WriteLine("Invalid entry for salary. Please enter a valid employee salary");
            }
            Salary = salary;
        }

        public override string ToString()
        {
            return "Employee ID : " + Id + "\nName : " + Name + "\nAge : " + Age + "\nSalary : " + Salary;
        }

        public int CompareTo(Employee? other)
        {
            return this.Id.CompareTo(other?.Id);
        }

        public bool Equals(Employee? other)
        {
            return this.Id == other?.Id;
        }
    }
}
