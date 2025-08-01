using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;
using ShopOnline.Services;
using ShopOnline.Helper;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private const string CartKey = "Cart";

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment()
        {
            var baseUrl = $"http://localhost:4200/order";
            var cartItems = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            
            if (!cartItems.Any()) return NotFound(new { message = "No cart items" });

            var redirectUrl = await _paymentService.CreatePaymentAsync(cartItems, baseUrl);
            return Ok(new { redirectUrl });
        }

        [HttpGet("execute")]
        public async Task<IActionResult> ExecutePayment(string payerId, string paymentId)
        {
            var success = await _paymentService.ExecutePaymentAsync(payerId, paymentId);
            if (success)
                return Ok("Payment success");
            return BadRequest("Payment failed");
        }
    }

}