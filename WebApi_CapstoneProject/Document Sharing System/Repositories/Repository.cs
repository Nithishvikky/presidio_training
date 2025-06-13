using DSS.Contexts;
using DSS.Interfaces;

namespace DSS.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly DssContext _dssContext;

        public Repository(DssContext dssContext)
        {
            _dssContext = dssContext;
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();
        public async Task<T> Add(T item)
        {
            _dssContext.Add(item);
            await _dssContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
            {
                throw new KeyNotFoundException("No such item found for deleting");
            }
            _dssContext.Remove(item);
            await _dssContext.SaveChangesAsync();
            return item;
            
        }

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem == null)
            {
                throw new KeyNotFoundException("No such item found for updation");
            }
            _dssContext.Entry(myItem).CurrentValues.SetValues(item);
            await _dssContext.SaveChangesAsync();
            return item;
        }
    }
}