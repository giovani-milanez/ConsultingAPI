using Database.Model;
using Database.Model.Context;
using Database.Repository.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository
{
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        public FileRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task DeleteFileByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            var file = _context.Files.Where(x => x.Guid == guidBytes).FirstOrDefault();
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        public bool FileExists(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            return _context.Files.Any(x => x.Guid == guidBytes);
        }

        public Task<File> GetFileDetailsByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            var file = _context.Files.Where(x => x.Guid == guidBytes).Select(x => new File { UploaderId = x.UploaderId }).FirstOrDefault();
            return Task.FromResult(file);
        }

        public Task<File> GetFileByGuidAsync(Guid fileGuid)
        {
            var guidBytes = fileGuid.ToByteArray();
            var file = _context.Files.Where(x => x.Guid == guidBytes).FirstOrDefault();
            return Task.FromResult(file);
        }

        //public async Task<File> SaveFileAsync(File file, User requester)
        //{
        //    await base.CreateAsync(file);
                        
        //    var user = _context.Users.Where(x => x.Id == requester.Id).FirstOrDefault();
        //    if (user != null)
        //    {
        //        user.ProfilePicture = file.Id;
        //        await _context.SaveChangesAsync();
        //    }

        //    return file;
        //}
    }
}
