using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Misc
{
    public class UserDocMapper
    {
        public UserDocDetailDto MapUserDoc(UserDocument doc)
        {
            UserDocDetailDto userDocDetail = new UserDocDetailDto();
            userDocDetail.FileName = doc.FileName;
            userDocDetail.DocId = doc.Id;
            userDocDetail.ContentType = doc.ContentType;
            userDocDetail.UploadedAt = doc.UploadedAt;
            userDocDetail.UploaderEmail = doc.UploadedByUser.Email;
            if (doc.FileData != null)
                userDocDetail.Size = doc.FileData.Length;

            return userDocDetail;
        }
    }
}