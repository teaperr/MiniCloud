using MiniCloudServer.Services.Interfaces;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiniCloudServer.Services
{
    public interface IDirectoryService : IService
    {
        void CreateDirectory(string userName, string path, string directoryName);
        void CreateUserDirectory(string userName);
        Task<XDocument> GetDirectoryStructure(string userName);
        void RemoveDirectory(string userName, string path);
    }
}