using System;
using System.Diagnostics;
using System.Security.Cryptography;


namespace Day12
{
    internal class Task1
    {
        static string UserInputValidation()
        {
            string? input;
            while (String.IsNullOrEmpty(input = Console.ReadLine()?.Trim()))
            {
                Console.Write("Enter valid word : ");
            }

            return input;
        }
        static int UserInputNumValidation()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Enter valid number : ");
            }
            return num;
        }
        static void AddPostsLikes(object[][] users, int i)
        {
            Console.Write("\nEnter number of posts : ");
            int p = UserInputNumValidation();

            string[] posts = new string[p];
            int[] likes = new int[p];

            for (int j = 0; j < p; j++)
            {
                Console.Write("Please, Enter the caption : ");
                posts[j] = UserInputValidation();
                Console.Write("Please, Enter the respective likes for the post : ");
                likes[j] = UserInputNumValidation();
            }


            users[i] = new object[] {posts,likes};
        }
        
        static void Display(object[][] users,int i)
        {
            Console.WriteLine($"\nSummary of User {i+1}");

            string[] captions = (string[])users[i][0];
            int[] likes = (int[])users[i][1];

            for(int j=0;j<captions.Length;j++)
            {
                Console.Write($"Post {j+1} - Caption : {captions[j]} | Likes : {likes[j]}");
                Console.WriteLine();
            }
        }
        public void Run()
        {
            Console.Write("Please, Enter number of users : ");
            int N = UserInputNumValidation();

            object[][] users = new object[N][];
            int currUser = 0;
            bool flag = true;

            while (flag)
            {
                Console.WriteLine("\nChoose from Menu\n\t1. Add user\n\t2. Display Last User\n\t3. Exit");
                Console.Write("\nEnter the option from menu : ");
                int op = UserInputNumValidation();
                switch (op)
                {
                    case 1: if(currUser < N)
                            {
                                AddPostsLikes(users,currUser);
                                currUser++;
                            }
                            else
                            {
                                Console.WriteLine("User slot filled...Can't add users");
                            }
                            break;
                    case 2: if (currUser > 0) 
                            { 
                                Display(users, currUser - 1); 
                            }
                            else 
                            { 
                                Console.WriteLine("No user exists...Try to add users"); 
                            }
                            break;
                    case 3: flag = false;
                            break;
                    default: Console.WriteLine("Please ensure the option number...");
                             break;
                }

            }
        }
    }
}
