using DSS.Models.DTOs;

namespace DSS.Models.DTOs
{
    public class TopSharedDocumentDto
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; }
        public string Owner { get; set; }
        public int ShareCount { get; set; }
    }
}
