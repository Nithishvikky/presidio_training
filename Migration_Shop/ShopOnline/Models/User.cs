namespace ShopOnline.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Read-only no setter method
        public ICollection<News> News { get; } = new List<News>();

        // Read-only no setter method
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}