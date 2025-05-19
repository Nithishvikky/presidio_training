using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    internal class Tasks2to5
    {
        static int Largest(int a, int b)
        {
            return a > b ? a : b;
        }
        static void Calc(int a, int b)
        {
            Console.WriteLine("Enter the operation wants to do(*/+-) : ");
            char op = Convert.ToChar(Console.ReadLine());
            switch (op)
            {
                case '+':
                    Console.WriteLine($"Result : {a + b}");
                    break;
                case '-':
                    Console.WriteLine($"Result : {a - b}");
                    break;
                case '*':
                    Console.WriteLine($"Result : {a * b}");
                    break;
                case '/':
                    Console.WriteLine($"Result : {a / b}");
                    break;
                default:
                    Console.WriteLine("Enter valid Operator");
                    break;
            }
        }
        static void userCheck()
        {
            int attempt = 3;
            while (attempt > 0)
            {
                Console.WriteLine("Enter username : ");
                string? username = Console.ReadLine();

                if (!string.IsNullOrEmpty(username) && username == "Admin")
                {
                    Console.WriteLine("Enter password : ");
                    string? password = Console.ReadLine();

                    if (!string.IsNullOrEmpty(password) && password == "pass")
                    {
                        Console.WriteLine("Successfully Logged in");
                        attempt = 0;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Check credential...Password invalid");
                        attempt--;
                    }
                }
                else
                {
                    Console.WriteLine("Check credential...Username invalid");
                    attempt--;
                }

                if (attempt == 0)
                {
                    Console.WriteLine("You failed 3 attempts");
                }
            }
        }
        static void CountingNumbers()
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Enter Number {i + 1} : ");
                int a = Convert.ToInt32(Console.ReadLine());
                if (a % 7 == 0)
                {
                    count++;
                }
            }
            Console.WriteLine($"Count (divisible by 7) : {count}");
        }
        public void Run()
        {
            Console.WriteLine("Enter num1 : ");
            int num1 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter num2 : ");
            int num2 = Convert.ToInt32(Console.ReadLine());

            // Task2 input
            int MaxNum = Largest(num1,num2);
            Console.WriteLine($"Largest number from {num1} & {num2} is {MaxNum}");

            //Task3 input
            Calc(num1,num2);

            //Task4 input
            userCheck();
            
            //Task5 input
            CountingNumbers();

        }
    }
}
