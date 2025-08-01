using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class ShoppingCartService
    {
        private readonly ShopOnlineContext _context;
        private readonly IPaymentService _paymentService;

        public ShoppingCartService(ShopOnlineContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<List<Cart>> AddToCart(List<Cart> currentCart, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return currentCart;

            var existing = currentCart.FirstOrDefault(x => x.ProductId == productId);

            if (existing != null)
                existing.Quantity++;
            else
                currentCart.Add(new Cart { ProductId = productId, Quantity = 1 });

            return currentCart;
        }

        public List<Cart> DeleteFromCart(List<Cart> currentCart, int productId)
        {
            var existing = currentCart.FirstOrDefault(x => x.ProductId == productId);
            if (existing != null)
                currentCart.Remove(existing);

            return currentCart ?? [];
        }

        public List<Cart> UpdateCart(List<Cart> cart, string[] quantities)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                cart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            return cart;
        }

        public async Task ProcessOrder(List<Cart> CartItems,Order order)
        {
            foreach (var cart in CartItems)
            {
                var product = await _context.Products.FindAsync(cart.ProductId);
                if (product != null)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = cart.ProductId,
                        Quantity = cart.Quantity,
                        Price = product.Price
                    };
                    _context.OrderDetails.Add(detail);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"OrderDetail id {detail.OrderID}");
                }
            }
        }

        public async Task<Order> ProcessOrderAsync(List<Cart> CartItems, CustomerDto customer)
        {
            var orderDto = new OrderDto
            {
                OrderDate = DateTime.UtcNow,
                PaymentType = "Cash",
                Status = "Processing",
                Customer = customer
            };

            var order = new OrderMapper().MapOrder(orderDto);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await ProcessOrder(CartItems, order); // To add in orderdetails

            return order;
        }
        
        public async Task<string> ProcessOrderByPayPalAsync(List<Cart> CartItems,CustomerDto customer)
        {
            var orderDto = new OrderDto
            {
                OrderDate = DateTime.UtcNow,
                PaymentType = "PayPal",
                Status = "Processing",
                Customer = customer
            };

            var order = new OrderMapper().MapOrder(orderDto);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            await ProcessOrder(CartItems, order);

            var baseUrl = $"http://localhost:4200/payment-success";

            var redirectUrl = await _paymentService.CreatePaymentAsync(CartItems,baseUrl);

            return redirectUrl;
        }
    }
}