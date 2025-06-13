using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IUserDocService
    {
        public Task<UserDocument> UploadDoc(UserDocument doc);
        public Task<UserDocument> GetByFileName(string filename, string UploaderEmail);
        public Task<ICollection<UserDocument>> GetAllUserDocs(Guid userId);
        public Task<UserDocument> DeleteByFileName(string filename,Guid userId);
        
    }
}