namespace ShopOnline.DTOs
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string? Status { get; set; }
        public CustomerDto Customer { get; set; }
        public int ProductId { get; set; }
        public int Quantity{ get; set; }
    }
}