using MiniCloudServer.Exceptions;
using MiniCloudServer.Services.Interfaces;
using MiniCloudServer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    class FileService : IFileService
    {
        private readonly IResourceAccessService _resourceAccessService;

        public FileService(IResourceAccessService resourceAccessService)
        {
            _resourceAccessService = resourceAccessService;
        }

        public async Task<byte[]> DownloadFile(string userName, string ownerName, string filePath)
        {
            var usersWithAccess=_resourceAccessService.ListUserWithAccessToResource(ownerName,filePath);
            if(!usersWithAccess.Contains(userName))
                throw new MiniCloudException("User doesn't have access to resource");
            string fileServerPath = PathUtilities.GenerateFullPath(ownerName, filePath);
            if (!File.Exists(fileServerPath))
                throw new MiniCloudException("File doesn't exists");
            var fileBytes= await File.ReadAllBytesAsync(fileServerPath);;
            return fileBytes;
        }

        public void RemoveFile(string userName, string filePath)
        {
            string fileServerPath = PathUtilities.GenerateFullPath(userName, filePath);
            if (!File.Exists(fileServerPath))
                throw new MiniCloudException("File doesn't exists");
            File.Delete(fileServerPath);
        }

        public async Task UploadFile(string userName, string directoryPath, string fileName, byte[] fileBytes)
        {
            string serverPath = PathUtilities.GenerateFullPath(userName, directoryPath);
            if (!Directory.Exists(serverPath))
                throw new MiniCloudException("Selected directory doesn't exists");
            string filePath=$"{serverPath}//{fileName}";
            if (File.Exists(serverPath))
                throw new MiniCloudException("File with this name already exists.");
            await File.WriteAllBytesAsync(filePath,fileBytes);
            
        }
    }
}
