namespace ShopOnline.DTOs
{
    public class ProductDto
    {
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int IsNew { get; set; } = 1; // default 1-new 2-old

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public int ModelId { get; set; }
        public int StorageId { get; set; } 
    }
}