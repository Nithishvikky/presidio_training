using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    internal class Task1
    {
        static void greet(string n)
        {
            Console.WriteLine($"Welcome {n}!!!");
        }
        public void Run()
        {
            Console.Write("Please enter your name : ");
            string name = Console.ReadLine();
            greet(name);
        }
    }
}
