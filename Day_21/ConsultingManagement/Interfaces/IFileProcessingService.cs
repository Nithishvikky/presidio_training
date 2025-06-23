using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IFileProcessingService
    {
        public Task<FileUploadReturnDTO> ProcessData(CsvUploadDto csvUploadDto);
    }
}