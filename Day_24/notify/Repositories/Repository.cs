using Notify.Contexts;
using Notify.Interfaces;

namespace Notify.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly NotifyContext _notifyContext;
        public Repository(NotifyContext notifyContext)
        {
            _notifyContext = notifyContext;
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();
        
        public async Task<T> Add(T item)
        {
            _notifyContext.Add(item);
            await _notifyContext.SaveChangesAsync();//generate and execute the DML quries for the objects whse state is in ['added','modified','deleted'],
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _notifyContext.Remove(item);
                await _notifyContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for deleting");
        }
        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _notifyContext.Entry(myItem).CurrentValues.SetValues(item);
                await _notifyContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for updation");
        }
    }

}