using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Sample
{
    internal class Task12
    {
        static bool ValidWord(string word)
        {
            foreach (char c in word) 
            {
                if (!char.IsLetter(c) || !char.IsLower(c))
                {
                    return false;
                }
            }
            return true;
        }
        static string UserInputValidation()
        {
            Console.Write("Please Enter the word : ");
            string? input = Console.ReadLine();
            while (String.IsNullOrEmpty(input?.Trim()) || !ValidWord(input))
            {
                Console.WriteLine("Enter valid word...");
                Console.Write("Please Enter the word : ");
                input = Console.ReadLine();
            }

            return input;
        }
        static string EncryptsWord(string input,int shift)
        {
            string res = "";
            foreach (char c in input)
            {
                int i = (int)c;
                i = (i - 97) + shift;
                i = (i % 26) + 97;
                res = res  + (char)i;
            }
            return res;
        }
        static string DecryptsWord(string input, int shift)
        {
            string res = "";
            foreach (char c in input)
            {
                int i = (int)c;
                i = (i - 97) - shift + 26;
                i = (i % 26) + 97;
                res = res + (char)i;
            }
            return res;
        }
        public void Run()
        {
            string input = UserInputValidation();

            Console.Write("\nEnter number of shift to encrypt : ");
            int shift = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            input = EncryptsWord(input, shift);

            Console.WriteLine($"Encrypted word : {input}");

            input = DecryptsWord(input, shift);

            Console.WriteLine($"Decrypted word : {input}");

        }
    }
}

//12) Write a program that:

//Takes a message string as input (only lowercase letters, no spaces or symbols).

//Encrypts it by shifting each character forward by 3 places in the alphabet.

//Decrypts it back to the original message by shifting backward by 3.

//Handles wrap-around, e.g., 'z' becomes 'c'.

//Examples
//Input:     hello
//Encrypted: khoor
//Decrypted: hello

//Test cases
//| Input | Shift | Encrypted | Decrypted |
//| ----- | ----- | --------- | --------- |
//| hello | 3     | khoor     | hello     |
//| world | 3     | zruog     | world     |
//| xyz   | 3     | abc       | xyz       |
//| apple | 1     | bqqmf     | apple     |