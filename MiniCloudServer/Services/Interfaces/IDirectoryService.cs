using System.Xml.Linq;

namespace MiniCloudServer.Services
{
    public interface IDirectoryService
    {
        void CreateDirectory(string userName, string path, string directoryName);
        void CreateUserDirectory(string userName);
        XDocument GetDirectoryStructure(string userName);
        void RemoveDirectory(string userName, string path);
    }
}