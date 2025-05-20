using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    internal class Easy
    {
        static void Addemployess(List<string> li)
        {
            string? name;
            Console.WriteLine("Please enter the employee names in the order of their eligibility for promotion");
            while (!String.IsNullOrWhiteSpace(name = Console.ReadLine()))
            {
                li.Add(name);
            }
        }
        static void DisplayEmployee(List<string> li)
        {
            Console.WriteLine(string.Join(", ", li));
        }
        static void FindEmployee(List<string> li)
        {
            Console.WriteLine("Please enter the name of the employee to check promotion position ");
            string? name = Console.ReadLine();
            if (li.Contains(name))
            {
                Console.WriteLine($"“{name}” is the the position {li.IndexOf(name)} for promotion. ");
            }
        }
        static void ModifySize(List<string> li)
        {
            Console.WriteLine($"The current size of the collection is {li.Capacity}");
            if(li.Count < li.Capacity)
            {
                li.TrimExcess();
            }

            Console.WriteLine($"The size after removing the extra space is {li.Capacity}"); 
        }
        static void PromotedList(List<string> li)
        {
            Console.WriteLine("Promoted employee list: ");
            List<string> temp = new List<string>(li);
            temp.Sort();
            Console.WriteLine(string.Join(", ",temp));
        }
        public void Run()
        {
            List<string> li = new List<string>();

            Addemployess(li);
            DisplayEmployee(li);
            //FindEmployee(li);
            //ModifySize(li);
            //PromotedList(li);
        }
    }
}
