using MiniCloudServer.Services.Interfaces;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    public interface IResourceAccessService : IService
    {
        Task ShareAccessToResourceAsync(string doneeName, string ownerName, string path);
    }
}