using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    internal class Tasks6to8
    {
        static void Print(int[] arr)
        {
            foreach (int i in arr)
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();
        }
        static void FrequencyChecker(int[] arr)
        {
            Dictionary<int, int> map = new Dictionary<int, int>(); // Key -> actual number Value -> count of number

            foreach (int i in arr)
            {
                if (map.ContainsKey(i))
                {
                    map[i]++;
                }
                else
                {
                    map[i] = 1;
                }
            }
            foreach (var item in map)
            {
                Console.WriteLine($"{item.Key} occurs {item.Value} times");
            }
        }
        static void RotateArray(int[] arr)
        {
            int temp = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                arr[i - 1] = arr[i];
            }
            arr[arr.Length - 1] = temp;
            Print(arr);
        }
        static void MergeArray(int[] arr1, int[] arr2)
        {
            int n = arr1.Length + arr2.Length;
            int[] res = new int[n];

            for (int i = 0; i < n; i++)
            {
                if (i < arr1.Length)
                {
                    res[i] = arr1[i];
                }
                else
                {
                    res[i] = arr2[i - arr1.Length];
                }
            }
            Print(res);
        }
        public void Run()
        {
            //int[] numbers = { 1, 2, 2, 3, 4, 4, 4 };
            //frequencyChecker(numbers);
            //int[] arrayForRotation = { 10, 20, 30, 40, 50 };
            //Print(arrayForRotation);
            //RotateArray(arrayForRotation);
            int[] array1 = { 1, 3, 5 };
            int[] array2 = { 2, 4, 6 };
            Print(array1);
            Print(array2);
            MergeArray(array1, array2);
        }
    }
}
