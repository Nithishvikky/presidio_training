using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Misc
{
    public class UserDocMapper
    {
        public UserDocDetailDto MapUserDoc(UserDocument doc)
        {
            UserDocDetailDto userDocDetail = new UserDocDetailDto();
            userDocDetail.DocId = doc.Id;
            userDocDetail.FileName = doc.FileName;
            userDocDetail.ContentType = doc.ContentType;
            userDocDetail.Status = doc.Status;
            userDocDetail.UploadedAt = doc.UploadedAt;
            userDocDetail.UploaderEmail = doc.UploadedByUser?.Email ?? "Unknown";
            userDocDetail.UploaderUsername = doc.UploadedByUser?.Username ?? "Unknown";
            if (doc.FileData != null)
                userDocDetail.Size = doc.FileData.Length;
            userDocDetail.ArchivedAt = doc.ArchivedAt;
            userDocDetail.IsDeleted = doc.IsDeleted;
            userDocDetail.TemporarilyUnarchivedAt = doc.TemporarilyUnarchivedAt;
            userDocDetail.ScheduledRearchiveAt = doc.ScheduledRearchiveAt;

            return userDocDetail;
        }
    }
}


