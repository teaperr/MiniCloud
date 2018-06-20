﻿using MiniCloudServer.Exceptions;
using MiniCloudServer.Services.Interfaces;
using MiniCloudServer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Services
{
    class FIleService : IFileService
    {
        public async Task<byte[]> DownloadFile(string userName, string filePath)
        {
            string fileServerPath = PathUtilities.GenerateFullPath(userName, filePath);
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
