using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Models;
using ShopOnline.Services;
using ShopOnline.Helper;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartService _cartService;
        private const string CartKey = "Cart";

        public ShoppingCartController(ShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            //Console.WriteLine(cart[0].ProductId);
            return Ok(cart);
        }

        [HttpPost("AddToCart/{id}")]
        public async Task<ActionResult> OrderNow(int id)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            cart = await _cartService.AddToCart(cart, id);
            HttpContext.Session.SetObject(CartKey, cart);
            return Ok(cart);
        }

        [HttpDelete("RemoveFromCart/{id}")]
        public ActionResult Delete(int id)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            cart = _cartService.DeleteFromCart(cart, id);
            HttpContext.Session.SetObject(CartKey, cart);
            return Ok(cart);
        }

        [HttpPost("UpdateCart")]
        public IActionResult UpdateCart([FromForm] string[] quantity)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            cart = _cartService.UpdateCart(cart, quantity);
            HttpContext.Session.SetObject(CartKey, cart);
            return Ok(cart);
        }

        [HttpPost("Order/COD")]
        public async Task<IActionResult> ProcessOrderCod([FromBody] CustomerDto dto)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            if (!cart.Any()) NotFound("No items in cart");

            var order = await _cartService.ProcessOrderAsync(cart, dto);
            HttpContext.Session.Remove(CartKey);
            return Ok(new { OrderId = order.OrderID, Message = "Order placed successfully." });
        }

        [HttpPost("Order/Paypal")]
        public async Task<IActionResult> ProcessOrderPaypal([FromBody] CustomerDto dto)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>(CartKey) ?? new List<Cart>();
            if (!cart.Any()) NotFound("No items in cart");
            
            var redirectUrl = await _cartService.ProcessOrderByPayPalAsync(cart,dto);
            HttpContext.Session.Remove(CartKey);
            return Ok(new { redirectUrl });
        }
    }
}