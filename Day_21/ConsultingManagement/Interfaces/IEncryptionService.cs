using ConsultingManagement.Models;

namespace ConsultingManagement.Interfaces
{
    public interface IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data);
    }
}