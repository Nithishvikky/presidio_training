using ConsultingManagement.Contexts;
using ConsultingManagement.Interfaces;

namespace ConsultingManagement.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly ConsultancyContext _consultancyContext;
        public Repository(ConsultancyContext consultancyContext)
        {
            _consultancyContext = consultancyContext;
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();
        
        public async Task<T> Add(T item)
        {
            _consultancyContext.Add(item);
            await _consultancyContext.SaveChangesAsync();//generate and execute the DML quries for the objects whse state is in ['added','modified','deleted'],
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item != null)
            {
                _consultancyContext.Remove(item);
                await _consultancyContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for deleting");
        }
        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _consultancyContext.Entry(myItem).CurrentValues.SetValues(item);
                await _consultancyContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for updation");
        }
    }

}