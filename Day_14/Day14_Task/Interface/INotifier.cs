using Day14_Task.Model;

namespace Day14_Task.Interface
{
    // Ensuring the OCP...Open for extension and closes for modification
    public interface INotifier
    {
        void Send(User user);
    }
}
