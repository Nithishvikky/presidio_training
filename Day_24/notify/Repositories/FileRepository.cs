using Notify.Contexts;
using Notify.Models;
using Microsoft.EntityFrameworkCore;

namespace Notify.Repositories
{
    public class FileRepository : Repository<int, UploadedFile>
    {
        public FileRepository(NotifyContext context) : base(context) { }
        
        public override async Task<UploadedFile> Get(int key)
        {
            return await _notifyContext.UploadedFiles.SingleOrDefaultAsync(u => u.Id == key);
        }

        public override async Task<IEnumerable<UploadedFile>> GetAll()
        {
            return await _notifyContext.UploadedFiles.ToListAsync();
        }
           
    }
}