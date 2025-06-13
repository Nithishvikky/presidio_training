namespace DSS.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        public Task<T> Add(T item);
        public Task<T> Update(K key, T item);
        public Task<T> Delete(K key);
        public Task<T> Get(K key);
        public Task<IEnumerable<T>> GetAll();
    
    }
}