using Database.Model;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IFileRepository : IRepository<File>
    {
        Task<File> GetFileDetailsByIdAsync(long fileId);
        Task<File> GetFileDetailsByGuidAsync(System.Guid fileGuid);
        Task<File> GetFileByGuidAsync(System.Guid fileGuid);
        Task DeleteFileByGuidAsync(System.Guid fileGuid);
        bool FileExists(System.Guid fileGuid);
    }
}
