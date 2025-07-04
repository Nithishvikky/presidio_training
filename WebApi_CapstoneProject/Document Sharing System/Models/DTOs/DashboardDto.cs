namespace DSS.Models.DTOs
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalAdmin{ get; set; }
        public int TotalUserRole{ get; set; }
        public int TotalDocuments { get; set; }
        public int TotalShared { get; set; }
        public int TotalViews{ get; set; }
    }
}