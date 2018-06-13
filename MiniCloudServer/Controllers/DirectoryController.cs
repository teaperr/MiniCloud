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
        private readonly IAccountService _accountService;
        private readonly IResourceAccessService _resourceAccessService;

        public DirectoryController(Session session): base(session)
        {
            _directoryService =new DirectoryService();
            var dbContext = new MiniCloudContext();
            var encryptService = new EncryptService();
            _accountService = new AccountService(dbContext, encryptService, session);
            _resourceAccessService=new ResourceAccessService();
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
            var structure=await _directoryService.GetDirectoryStructure(userName);
            return structure.ToString();
        }
        public async Task<string> Remove(string path)
        {
            var userName = (await _accountService.GetLoggedUser()).UserName;
            _directoryService.RemoveDirectory(userName,path);
            return "Removed";
        }
        public async Task<string> Share(string doneeName, string path)
        {
            var userName = (await _accountService.GetLoggedUser()).UserName;
            await _resourceAccessService.ShareAccessToResourceAsync(doneeName,userName,path);
            return "OK";
        }
    }
}
