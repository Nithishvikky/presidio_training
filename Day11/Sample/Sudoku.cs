using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    internal class Sudoku
    {
        private HashSet<int>h = new HashSet<int>();

        static int UserInputValidation()
        {
            int num;
            Console.Write("Please enter the number : ");
            while (!int.TryParse(Console.ReadLine(), out num) || num < 1 || num > 9) {
                Console.WriteLine("\nInvalid Input...Please enter a number from 1 to 9\n");
                Console.Write("Please enter the number : ");
            }

            return num;
        }
        public void Run()
        {
            for (int i = 0; i < 9; i++)
            {
                int userInput = UserInputValidation();
                h.Add(userInput);
            }
            if (h.Count == 9)
            {
                Console.WriteLine("The Row is valid");
            }
            else
            {
                Console.WriteLine("The Row is invalid");
            }
        }
    }
}

//10) write a program that accepts a 9-element array representing a Sudoku row.

//Validates if the row:

//Has all numbers from 1 to 9.

//Has no duplicates.

//Displays if the row is valid or invalid.