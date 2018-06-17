using MiniCloudServer.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    public interface IResourceAccessService : IService
    {
        Task ShareAccessToResourceAsync(string doneeName, string ownerName, string path);
        Task StopShareAccessToResourceAsync(string doneeName, string ownerName, string path);
        IEnumerable<string> ListUserWithAccessToResource(string ownerName, string path);
    }
}