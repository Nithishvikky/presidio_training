namespace ShopOnline.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // Read-only no setter method
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}