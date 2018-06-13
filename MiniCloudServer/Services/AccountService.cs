using Microsoft.EntityFrameworkCore;
using MiniCloud.Core;
using MiniCloud.Entities;
using MiniCloudServer.Core;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServer.Services
{
    public class AccountService: IAccountService
    {
        private readonly MiniCloudContext _dbContext;
        private readonly IEncryptService _encryptService;
        private readonly Session _session;
        private readonly IDirectoryService _directoryService;

        public AccountService(MiniCloudContext dbContext, IEncryptService encryptService, Session session)
        {
            _dbContext = dbContext;
            _encryptService=encryptService;
            _session = session;
            _directoryService=new DirectoryService();
        }
        public async Task RegisterUserAsync(string userName, string password)
        {
            var user=await _dbContext.Users.SingleOrDefaultAsync(x=>x.UserName==userName);
            if(user!=null)
                throw new MiniCloudException("User with this name already exists");
            user=new User(userName,password);
            await _dbContext.Users.AddAsync(user);
            _directoryService.CreateUserDirectory(userName);
            await _dbContext.SaveChangesAsync();
        }
        public async Task LoginUserAsync(string userName, string password)
        {
            var user=await _dbContext.Users.SingleOrDefaultAsync(x=>x.UserName==userName);
            if(user==null)
                throw new MiniCloudException("Invalid Credentials");
            var hashedPassword=_encryptService.Compute(password,user.Salt);
            if(user.HashedPassword!=hashedPassword)
                throw new MiniCloudException("Invalid Credentials");
            _session.AddObject("logged_user_id",user.Id);
        }
        public Task<User> GetLoggedUser()
        {
            if (!_session.TryGetValue("logged_user_id", out object loggedUserIdObject))
                throw new MiniCloudException("No one is logged");
            var user=_dbContext.Users.SingleOrDefault(x=>x.Id==(int)loggedUserIdObject);
            return Task.FromResult(user);
        }
    }
}
