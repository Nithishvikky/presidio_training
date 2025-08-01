using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class OrderDetailService : IOrderdetailService
    {
        private readonly ShopOnlineContext _context;
        public OrderDetailService(ShopOnlineContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail?> AddOrderDetail(OrderDetail orderDetail)
        {
            var product = await _context.Products.FindAsync(orderDetail.ProductID);

            if (product == null) throw new KeyNotFoundException("Product not found");

            orderDetail.Price = product.Price * orderDetail.Quantity;

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<bool> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return false;

            _context.OrderDetails.Remove(orderDetail);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}