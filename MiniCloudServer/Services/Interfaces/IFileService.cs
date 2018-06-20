using MiniCloudServer.Services.Interfaces;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    public interface IFileService: IService
    {
        Task<byte[]> DownloadFile(string userName, string filePath);
        void RemoveFile(string userName, string filePath);
        Task UploadFile(string userName, string directoryPath, string fileName, byte[] fileBytes);
    }
}