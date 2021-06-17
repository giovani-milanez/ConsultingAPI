using Database.Model;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IFileRepository : IRepository<FileDetail>
    {
        Task<FileDetail> GetFileDetailsByGuidAsync(System.Guid fileGuid);
        Task<FileDetail> GetFileWithContentByGuidAsync(System.Guid fileGuid);
        Task DeleteFileByGuidAsync(System.Guid fileGuid);
        bool FileExists(System.Guid fileGuid);
    }
}
