namespace ShopOnline.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public string Color1 { get; set; }

        // Read-only no setter method
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}