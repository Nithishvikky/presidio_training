using Bank.Contexts;
using Bank.Interfaces;

namespace Bank.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly BankContext _bankContext;
        public Repository(BankContext bankContext)
        {
            _bankContext = bankContext;
        }

        public abstract Task<T?> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();
        
        public async Task<T> Add(T item)
        {
            _bankContext.Add(item);
            await _bankContext.SaveChangesAsync();//generate and execute the DML quries for the objects whse state is in ['added','modified','deleted'],
            return item;
        }

        public async void Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _bankContext.Remove(item);
                await _bankContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item Not found");
            }
        }
        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _bankContext.Entry(myItem).CurrentValues.SetValues(item);
                await _bankContext.SaveChangesAsync();
                return myItem;
            }
            throw new Exception("Item Not found");
        }
    }

}