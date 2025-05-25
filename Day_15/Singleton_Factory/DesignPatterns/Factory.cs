using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singleton_Factory.DesignPatterns
{
    // Product Interface
    public interface IFileHandler 
    {
        // Can be read or write...For read, there is no need for content so it can be null
        void Handle(string filePath, string content = null);
    }

    // Concrete Product classes
    public class FileReader : IFileHandler
    {
        public void Handle(string filePath, string content = null)
        {
            Console.WriteLine("Read method of Factory");
            using (StreamReader _reader = new StreamReader(filePath))
            {
                Console.WriteLine(_reader.ReadToEnd());
            }
            
        }
    }
    public class FileWriter : IFileHandler
    {
        public void Handle(string filePath, string content = null)
        {
            Console.WriteLine("Write method of Factory");
            using (StreamWriter _writer = new StreamWriter(filePath))
            {
                _writer.WriteLine(content);
            }
            Console.WriteLine("Write method Successful");
        }
    }

    // Abstract Creator - Factory class
    public abstract class FileHandleCreator
    {
        //IFileHandler - so it could be FileWriter/FileReader
        public abstract IFileHandler CreateFileHandler();  

        public void Process(string filePath,string content = null)
        {
            IFileHandler fileHandler = CreateFileHandler(); // Factory method - don't know what handler it will return (read/write)
            fileHandler.Handle(filePath, content);
        } 
    }

    // Concrete Creator classes
    public class ReadFileCreator : FileHandleCreator
    {
        // This method override the abstract creator - return one of IFileHandler
        public override IFileHandler CreateFileHandler()
        {
            Console.WriteLine("CreateFileHandler() Called....(Reader Creator Class)");
            return new FileReader(); // return the Reader Product class
        }
    }

    public class WriteFileCreator : FileHandleCreator
    {
        public override IFileHandler CreateFileHandler()
        {
            Console.WriteLine("CreateFileHandler() Called....(Reader Creator Class)");
            return new FileWriter(); // return the writer product class
        }
    }
}
