namespace ShopOnline.Models
{
    public class Model
    {
        public int ModelId { get; set; }
        public string Model1 { get; set; }

        // Read-only no setter method
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}