using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    public interface IResourceAccessService
    {
        Task ShareAccessToResourceAsync(string doneeName, string ownerName, string path);
    }
}