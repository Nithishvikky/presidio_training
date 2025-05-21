

namespace Day13_Task.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        private string _message = "Duplicate entity found";

        // Custom Exception
        public DuplicateEntityException(string msg)
        {
            _message = msg;
        }
        public override string Message => _message;
    }
}
