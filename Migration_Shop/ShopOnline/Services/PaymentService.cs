using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;
using PayPal.Api;
using ShopOnline.Helper;


namespace ShopOnline.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IProductService _productService;

        public PaymentService(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<string> CreatePaymentAsync(List<Cart> cartItems, string redirectUrl)
        {
            var apiContext = PaypalConfiguration.GetAPIContext();

            var listItems = new ItemList { items = new List<Item>() };
            double subtotal = 0;

            foreach (var cart in cartItems)
            {
                var product = await _productService.ProductDetails(cart.ProductId);
                if (product == null) continue;

                listItems.items.Add(new Item
                {
                    name = product.ProductName,
                    currency = "USD",
                    price = product.Price?.ToString("F2") ?? "0.00",
                    quantity = cart.Quantity.ToString(),
                    sku = $"sku_{product.ProductId}"
                });

                subtotal += (product.Price ?? 0) * cart.Quantity;
            }

            var payer = new Payer { payment_method = "paypal" };

            var redirUrls = new RedirectUrls
            {
                cancel_url = $"{redirectUrl}?success=false",
                return_url = $"{redirectUrl}?success=true"
            };

            var details = new Details
            {
                tax = "1",
                shipping = "2",
                subtotal = subtotal.ToString("F2")
            };

            var amount = new Amount
            {
                currency = "USD",
                total = (subtotal + 1 + 2).ToString("F2"),
                details = details
            };

            var transactionList = new List<Transaction>
            {
                new Transaction
                {
                    description = "ShopOnline Purchase",
                    invoice_number = Guid.NewGuid().ToString("N"),
                    amount = amount,
                    item_list = listItems
                }
            };

            var payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            var createdPayment = payment.Create(apiContext);
            return createdPayment.links.First(x => x.rel == "approval_url").href;
        }

        public async Task<bool> ExecutePaymentAsync(string payerId, string paymentId)
        {
            var apiContext = PaypalConfiguration.GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            var executedPayment = payment.Execute(apiContext, paymentExecution);
            return executedPayment.state.Equals("approved", StringComparison.OrdinalIgnoreCase);
        }
    }
}