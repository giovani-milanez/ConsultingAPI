using Database.Model;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IFileRepository : IRepository<File>
    {
        Task<File> GetFileDetailsByGuidAsync(System.Guid fileGuid);
        Task<File> GetFileByGuidAsync(System.Guid fileGuid);
        //Task<File> SaveFileAsync(File file, User requester);
        Task DeleteFileByGuidAsync(System.Guid fileGuid);
        bool FileExists(System.Guid fileGuid);
    }
}
