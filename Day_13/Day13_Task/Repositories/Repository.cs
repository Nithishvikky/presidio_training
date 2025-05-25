using Day13_Task.Exceptions;
using Day13_Task.Interfaces;

namespace Day13_Task.Repositories
{
    public abstract class Repository<K, T> : IRepositor<K, T> where T : class
    {
        protected List<T> _items = new List<T>();
        protected abstract K GenerateID();
        public abstract ICollection<T> GetAll();
        public abstract T GetById(K id);

        public T Add(T item)
        {
            var id = GenerateID();
            var property = typeof(T).GetProperty("Id");
            if (property != null)
            {
                property.SetValue(item, id);
            }
            _items.Add(item);
            return item;
        }

        public void Delete(K id)
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new KeyNotFoundException("Item not found");
            }
            _items.Remove(item);
        }

        public void Update(T item)
        {
            var myItem = GetById((K)item.GetType().GetProperty("Id").GetValue(item));
            if (myItem == null)
            {
                throw new KeyNotFoundException("Item not found");
            }
            var index = _items.IndexOf(myItem);
            _items[index] = item;
        }
    }

}
