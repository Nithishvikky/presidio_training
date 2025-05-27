namespace ConsultingManagement.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        T Add(T item);
        T GetById(K id);
        ICollection<T> GetAll();
    }
}
