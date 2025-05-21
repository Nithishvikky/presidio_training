

namespace Day13_App.Interfaces
{
    public interface IRepositor<K, T> where T : class
    {
        T Add(T item);
        T Update(T item);
        T Delete(K id);
        T GetById(K id);
        ICollection<T> GetAll();
    }
}
