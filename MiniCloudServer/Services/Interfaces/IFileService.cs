using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MiniCloudServer.Services.Interfaces
{
    public interface IFileService
    {
        void UploadFile(string userName, byte[] fileBytes, string folderPath);
        byte[] DownloadFile(string userName, string path);
        void RemoveFile(string userName, string path);

    }
}
