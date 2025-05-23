namespace Singleton_Factory.DesignPatterns
{
    public class Singleton
    {
        private static Singleton _instance; // field to store instance
        private FileStream _fileStream;

        // Private construtor
        private Singleton(string path)
        {
            _fileStream = new FileStream(path, FileMode.OpenOrCreate,FileAccess.ReadWrite);
            Console.WriteLine("Singleton Instance created"); 
        } 
        public static Singleton GetInstance(string path) // static method to access
        {
            if (_instance == null)
            {
                _instance = new Singleton(path);
            }
            return _instance;
        }

        public void Write(string str)
        {
            Console.WriteLine("Write method of Singleton");
            StreamWriter _writer = new StreamWriter(_fileStream);
            _writer.WriteLine(str);

            Console.WriteLine("Write method Successful");
        }

        public string Read()
        {
            Console.WriteLine("Read method of Singleton");
            StreamReader _reader = new StreamReader(_fileStream);
            return _reader.ReadToEnd();
        }

        public void Close()
        {
            _fileStream?.Close();
        }
    }
}
