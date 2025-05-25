using Singleton_Factory.DesignPatterns;
using System.IO;


namespace Singleton_Factory
{
    public class Program
    {
        public static string path = "C:\\Users\\DELL\\Desktop\\Work\\Day_15\\Singleton_Factory\\file.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("\n-------------Singleton Design-------------\n");
            var singleton = Singleton.GetInstance(path);
            singleton.Write("Hello I am nithish");
            Console.WriteLine(singleton.Read());
            singleton.Close();

            Console.WriteLine("\n-------------Factory Design-------------\n");
            FileHandleCreator writer = new WriteFileCreator();
            writer.Process(path, "Nithish");

            FileHandleCreator reader = new ReadFileCreator();
            reader.Process(path);
        }
    }
}
