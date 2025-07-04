using DSS.Models.DTOs;

namespace DSS.Models.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ErrorObjectDto? Error { get; set; }
    }
}