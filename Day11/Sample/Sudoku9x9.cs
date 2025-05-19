using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    internal class Sudoku9x9
    {
        int[,] board = {
                { 2, 5, 7, 4, 9, 3, 1, 6, 8},
                { 3, 9, 4, 1, 8, 6, 7, 2, 5},
                { 6, 8, 1, 7, 5, 2, 4, 3, 9},
                { 1, 6, 9, 8, 2, 7, 5, 4, 3},
                { 7, 2, 8, 5, 3, 4, 9, 1, 6},
                { 4, 3, 5, 9, 6, 1, 8, 7, 2},
                { 9, 1, 3, 6, 7, 8, 2, 5, 4},
                { 8, 7, 6, 2, 4, 5, 3, 9, 1},
                { 5, 4, 2, 3, 1, 9, 6, 8, 7}
        };
        static bool Checker(int[] arr)
        {
            HashSet<int> s = new HashSet<int>();
            foreach (int i in arr) 
            { 
                s.Add(i);
            }
            return s.Count == arr.Length;
        }
        static bool RowIterator(int[,] arr)
        {
            for(int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                for(int j = 0; j < 9; j++)
                {
                    row[j] = arr[i, j];
                }

                if (!Checker(row))
                {
                    return false;
                }
            }
            return true;
        }
        static bool ColIterator(int[,] arr)
        {
            for (int i = 0; i < 9; i++)
            {
                int[] col = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    col[j] = arr[j, i];
                }

                if (!Checker(col))
                {
                    return false;
                }
            }
            return true;
        }
        static bool SubgridIterator(int[,] board)
        {
            for (int rowStart = 0; rowStart < 9; rowStart += 3)
            {
                for (int colStart = 0; colStart < 9; colStart += 3)
                {
                    int[] subgrid = new int[9];
                    int index = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            subgrid[index] = board[rowStart + i, colStart + j];
                            index++;
                        }
                    }

                    if (!Checker(subgrid))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Run()
        {
            if (RowIterator(board) && ColIterator(board) && SubgridIterator(board))
            {
                Console.WriteLine("Valid");
            }
            else
            {
                Console.WriteLine("Invalid");
            }
        }
    }
}
