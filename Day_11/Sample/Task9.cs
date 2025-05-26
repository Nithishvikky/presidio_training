using System;

namespace Sample
{
    internal class Task9
    {
        private const string guess = "GAME";
        static string UserInputValidation()
        {
            Console.Write("Please Enter your guess : ");
            string? input = Console.ReadLine();
            while (String.IsNullOrEmpty(input?.Trim()) || input?.Length != 4)
            {
                Console.WriteLine("Enter valid guess(4 letters)...");
                Console.Write("Please Enter your guess : ");
                input = Console.ReadLine();     
            }

            return input.ToUpper();
        }
        static bool CheckGuess(string input)
        {
            return input == guess ? true : false;
        }
        static void DisplayMessage(string input)
        {
            int bulls = 0, cows = 0;
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    if (input[i] == guess[j])
                    {
                        if (i == j)
                        {
                            bulls++;
                            break;
                        }
                        else
                        {
                            cows++;
                        }
                    }
                }
            }
            Console.WriteLine($"\nBulls : {bulls}");
            Console.WriteLine($"Cows : {cows}\n");
        }
        public void Run()
        {
            Console.WriteLine("Welcome to GuessGame!!!!\n");
            int attempt = 1;
            string UserGuess = UserInputValidation();
            while(!CheckGuess(UserGuess))
            {
                DisplayMessage(UserGuess);
                if(CheckGuess(UserGuess)) break;

                UserGuess = UserInputValidation();
                attempt++;
            }
            if (CheckGuess(UserGuess))
            {
                Console.WriteLine("\nYou guessed the word right!!!");
                Console.WriteLine($"Attempts : {attempt}");
            }
        }
    }
}
