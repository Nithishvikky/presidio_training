using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day15_Task.Interface;

namespace Day15_Task.Service
{
    public class File : IFile
    {
        private readonly string _filePath;

        public File(string filePath)
        {
            _filePath = filePath;
        }
        public void Read()
        {
            using(StreamReader Reader = new StreamReader(_filePath))
            {
                Console.WriteLine($"\n----Content of {new FileInfo(_filePath).Name}----\n");
                Console.Write(Reader.ReadToEnd());
                Console.WriteLine();
            }
        }
        public void MetaData()
        {
            var info  = new FileInfo( _filePath );
            Console.WriteLine($"\n----MetaData of {info.Name}----\n");
            Console.WriteLine($"File Name : {info.Name}");
            Console.WriteLine($"File Size : {info.Length} bytes");
            Console.WriteLine($"Created At : {info.CreationTime}");
            Console.WriteLine();

        }
    }
}
