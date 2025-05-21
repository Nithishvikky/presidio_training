

namespace Day13_Task.Interfaces
{
    public interface IRepositor<K, T> where T : class
    {
        T Add(T item);
        void Update(T item);
        void Delete(K id);
        T GetById(K id);
        ICollection<T> GetAll();
    }
}
