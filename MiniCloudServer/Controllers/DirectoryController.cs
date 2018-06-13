using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using MultiServer.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class DirectoryController:Controller
    {
        private readonly IDirectoryService _directoryService;
        private readonly AccountService _accountService;

        public DirectoryController(Session session): base(session)
        {
            _directoryService =new DirectoryService();
            var dbContext = new MiniCloudContext();
            var encryptService = new EncryptService();
            _accountService = new AccountService(dbContext, encryptService, session);
        }


        public async Task<string> Create(string path,string directoryName)
        {
            var userName=(await _accountService.GetLoggedUser()).UserName;
            _directoryService.CreateDirectory(userName,path,directoryName);
            return "Created";
        }
        public async Task<string> Structure()
        {
            var userName = (await _accountService.GetLoggedUser()).UserName;
            var structure=_directoryService.GetDirectoryStructure(userName).ToString();
            return structure;
        }
        public async Task<string> Remove(string path)
        {
            var userName = (await _accountService.GetLoggedUser()).UserName;
            _directoryService.RemoveDirectory(userName,path);
            return "Removed";
        }
    }
}
