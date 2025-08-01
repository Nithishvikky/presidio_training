using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IOrderService
    {
        public Task<Order?> AddOrder(Order order);
        public Task<Order?> GetOrderById(int id);
        public Task<bool> DeleteOrder(int id);
        public Task<Order?> UpdateOrder(Order order);
        
        public Task<PagedResult<Order>> OrderList(int? page);
        public Task<byte[]> ExportOrderListAsPdf();
    }
}