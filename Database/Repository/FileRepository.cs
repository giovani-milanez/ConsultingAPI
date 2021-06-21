using Database.Extension;
using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class FileRepository : GenericRepository<FileDetail>, IFileRepository
    {
        public FileRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task DeleteFileByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();

            bool deleted = false;
            // delete details
            var file = _context.FileDetails.Where(x => x.Guid == guidBytes).FirstOrDefault();
            if (file != null)
            {
                _context.FileDetails.Remove(file);
                deleted = true;
            }

            // delete content
            var content = _context.FileContents.Where(x => x.FileGuid == guidBytes).FirstOrDefault();
            if (content != null)
            {
                _context.FileContents.Remove(content);
                deleted = true;
            }

            if (deleted) await _context.SaveChangesAsync();
        }

        public bool FileExists(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            return _context.FileDetails.Any(x => x.Guid == guidBytes);
        }

        public Task<FileDetail> GetFileDetailsByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            var file = _context.FileDetails
                .AsNoTracking()
                .Where(x => x.Guid == guidBytes)
                .FirstOrDefault();
            return Task.FromResult(file);
        }

        public Task<FileDetail> GetFileWithContentByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            var file = _context.FileDetails.IncludeMultiple(nameof(FileDetail.Content)).AsNoTracking().Where(x => x.Guid == guidBytes).FirstOrDefault();
            return Task.FromResult(file);
        }
    }
}
