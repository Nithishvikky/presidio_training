using System.Data;
using ConsultingManagement.Contexts;
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        public readonly ConsultancyContext  _context;
        public FileProcessingService(ConsultancyContext  context)
        {
            _context = context;
        }

        public async Task<FileUploadReturnDTO> ProcessData(CsvUploadDto csvUploadDto)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM process_csv(:csv_input)";

            command.CommandType = CommandType.Text;

            var param = command.CreateParameter();
            param.ParameterName = "csv_input";
            param.Value = csvUploadDto.CsvContent;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();

            var errorRows = new List<string>();
            while (await reader.ReadAsync())
            {
                errorRows.Add(reader.GetString(0));
            }
            return new FileUploadReturnDTO{ Inserted = "CSV Processed", Errors = errorRows.ToArray() };
        }
    }
}