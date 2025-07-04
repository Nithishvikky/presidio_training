namespace DSS.Models.DTOs
{
    public class ViewerResponseDto
    {
        public Guid DocViewId { get; set; }
        public string ViewerName { get; set; } = string.Empty;
        public string ViewerEmail { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime ViewedAt{ get; set; }
    }
}