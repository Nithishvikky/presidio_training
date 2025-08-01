using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentAsync(List<Cart> cartItems, string redirectUrl);
        Task<bool> ExecutePaymentAsync(string payerId, string paymentId);
    }

}