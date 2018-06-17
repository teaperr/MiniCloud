﻿using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using MultiServer.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class DirectoryController:Controller
    {
        private readonly IDirectoryService _directoryService;
        private readonly IAccountService _accountService;
        private readonly IResourceAccessService _resourceAccessService;

        public DirectoryController(IDirectoryService directoryService, IAccountService accountService, IResourceAccessService resourceAccessService)
        {
            _directoryService = directoryService;
            _accountService = accountService;
            _resourceAccessService = resourceAccessService;
        }

        public async Task<string> Create(string path,string directoryName)
        {
            var userName=(await _accountService.GetLoggedUser(Session)).UserName;
            _directoryService.CreateDirectory(userName,path,directoryName);
            return "Created";
        }
        public async Task<string> Structure()
        {
            var userName = (await _accountService.GetLoggedUser(Session)).UserName;
            var structure=await _directoryService.GetDirectoryStructure(userName);
            return structure.ToString();
        }
        public async Task<string> Remove(string path)
        {
            var userName = (await _accountService.GetLoggedUser(Session)).UserName;
            _directoryService.RemoveDirectory(userName,path);
            return "Removed";
        }
        public async Task<string> Share(string doneeName, string path)
        {
            var userName = (await _accountService.GetLoggedUser(Session)).UserName;
            await _resourceAccessService.ShareAccessToResourceAsync(doneeName,userName,path);
            return "OK";
        }
        public async Task<string> StopShare(string doneeName, string path)
        {
            var userName = (await _accountService.GetLoggedUser(Session)).UserName;
            await _resourceAccessService.StopShareAccessToResourceAsync(doneeName, userName, path);
            return "OK";
        }
        public async Task<string> ListUsersWithAccess(string path)
        {
            var userName = (await _accountService.GetLoggedUser(Session)).UserName;
            var usersWithAccess=await _resourceAccessService.ListUserWithAccessToResource(userName,path);
            return String.Join(';',usersWithAccess);
        }
    }
}
