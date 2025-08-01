using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IOrderdetailService
    {
        public Task<OrderDetail?> AddOrderDetail(OrderDetail orderDetail);
        public Task<bool> DeleteOrderDetail(int id);
    }
}