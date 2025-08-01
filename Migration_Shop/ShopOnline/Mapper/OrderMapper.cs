using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using ShopOnline.DTOs;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Mapper
{
    public class OrderMapper
    {
        public Models.Order MapOrder(OrderDto orderDto)
        {
            Models.Order order = new Models.Order();

            order.OrderName = orderDto.Customer.CustomerName + "_"+orderDto.OrderDate.ToString("yyyyMMddHHmmss");
            order.OrderDate = orderDto.OrderDate;
            order.PaymentType = orderDto.PaymentType;
            order.Status = orderDto.Status ?? "Processing";

            // order.CustomerName = orderDto.Customer.CustomerName;
            // order.CustomerAddress = orderDto.Customer.CustomerAddress;
            // order.CustomerPhone = orderDto.Customer.CustomerPhone;
            // order.CustomerEmail = orderDto.Customer.CustomerEmail;
            order = MapCustomer(orderDto.Customer,order);

            return order;
        }

        public Models.Order MapCustomer(CustomerDto customerDto,Models.Order order)
        {

            order.CustomerName = customerDto.CustomerName;
            order.CustomerAddress = customerDto.CustomerAddress;
            order.CustomerPhone = customerDto.CustomerPhone;
            order.CustomerEmail = customerDto.CustomerEmail;

            return order;
        }

        public OrderDetail MapOrderToOrderDetail(OrderDto orderDto, int OrderId)
        {
            OrderDetail orderDetail = new OrderDetail();

            orderDetail.OrderID = OrderId;
            orderDetail.ProductID = orderDto.ProductId;
            orderDetail.Quantity = orderDto.Quantity;

            return orderDetail;
        }
    }
}