using MiniCloudServer.Services;
using MultiServer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService, IAccountService accountService): base(accountService)
        {
            _fileService = fileService;
        }

        public async Task<string> Download(string filePath)
        {
            var userName = await GetLoggedUserName();
            var fileBytes= (await _fileService.DownloadFile(userName, filePath));
            var base64File = Convert.ToBase64String(fileBytes);

            return base64File;
        }

        public async Task<string> Remove(string filePath)
        {
            var userName = await GetLoggedUserName();
            _fileService.RemoveFile(userName,filePath);
            return "Removed";
            
        }

        public async Task<string> Upload(string directoryPath, string fileName, string base64File)
        {
            var userName = await GetLoggedUserName();
            var fileBytes=Convert.FromBase64String(base64File);
            await _fileService.UploadFile(userName,directoryPath,fileName,fileBytes);
            return "Uploaded";

        }
    }
}
