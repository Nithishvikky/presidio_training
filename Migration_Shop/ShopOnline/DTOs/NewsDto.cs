namespace ShopOnline.DTOs
{
    public class NewsDto
    {
        public int UserId { get; set; }

        public string Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ImageUrl { get; set; }
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int Status { get; set; } = 1; // default 1 - active 2 - inactive
    }
}