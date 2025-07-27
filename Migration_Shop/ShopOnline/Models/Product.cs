namespace ShopOnline.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double? Price { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }
        public int? StorageId { get; set; }
        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? IsNew { get; set; }

        // Navigation properties

        public Category Category { get; set; }
        public Color Color { get; set; }
        public Model Model { get; set; }

        // Read-only no setter method
        public ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
        
        // Navigation property 
        public User User { get; set; }
    }
}