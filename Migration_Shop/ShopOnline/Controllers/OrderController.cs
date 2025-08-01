using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderdetailService _orderdetailService;

        public OrderController(IOrderService orderService, IOrderdetailService orderdetailService)
        {
            _orderService = orderService;
            _orderdetailService = orderdetailService;
        }


        [HttpGet]
        public async Task<ActionResult> OrderList(int page = 1)
        {
            var orders = await _orderService.OrderList(page);
            return Ok(orders);
        }

        // [HttpPost]
        // public async Task<ActionResult> AddOrder([FromBody] OrderDto orderDto)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     try
        //     {
        //         var order = new OrderMapper().MapOrder(orderDto);

        //         var created = await _orderService.AddOrder(order);

        //         if (created == null) throw new Exception("order process hasn't completed");

        //         var orderDetail = new OrderMapper().MapOrderToOrderDetail(orderDto, created.OrderID);
        //         orderDetail = await _orderdetailService.AddOrderDetail(orderDetail);

        //         if (orderDetail == null) throw new Exception("orderdetil process hasn't completed");

        //         return CreatedAtAction(nameof(GetOrderById), new { id = created?.OrderID }, created);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Conflict(ex);
        //     }
        // }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // [HttpDelete("Cancel/{id}")]
        // public async Task<ActionResult> DeleteById(int id)
        // {
        //     if (await _orderdetailService.DeleteOrderDetail(id))
        //     {
        //         if (await _orderService.DeleteOrder(id))
        //         {
        //             return Ok("Order and Order Details removed successfully");
        //         }
        //         return NotFound("Order not found");
        //     }
        //     return NotFound("order detail not found");
        // }

        // [HttpPut("Edit/{id}")]
        // public async Task<ActionResult> Edit(int id, [FromBody] CustomerDto customerDto)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);
        //     try
        //     {
        //         var order = await _orderService.GetOrderById(id);
        //         if (order == null) throw new KeyNotFoundException("Order not found");

        //         var UpdatingOrder = new OrderMapper().MapCustomer(customerDto, order);

        //         var UpdatedOrder = await _orderService.UpdateOrder(UpdatingOrder);

        //         return Ok(UpdatedOrder);
        //     }
        //     catch (KeyNotFoundException ex)
        //     {
        //         return NotFound(ex);
        //     }
        // }
        
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportOrderListing()
        {
            var pdfBytes = await _orderService.ExportOrderListAsPdf();
            var fileName = $"OrderListing_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}