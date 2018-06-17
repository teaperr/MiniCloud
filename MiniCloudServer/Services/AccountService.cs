using Microsoft.EntityFrameworkCore;
using MiniCloudServer.Core;
using MiniCloudServer.Entities;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using Server.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MultiServer.Services
{
    public class AccountService:IAccountService
    {
        private readonly MiniCloudContext _dbContext;
        private readonly IEncryptService _encryptService;
        private readonly IDirectoryService _directoryService;
        private readonly IResourceAccessService _resourceAccessService;

        public AccountService(MiniCloudContext dbContext, IEncryptService encryptService, IDirectoryService directoryService, IResourceAccessService resourceAccessService)
        {
            _dbContext = dbContext;
            _encryptService = encryptService;
            _directoryService = directoryService;
            _resourceAccessService = resourceAccessService;
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
            await _resourceAccessService.ShareAccessToResourceAsync(userName, userName, "");
        }
        public async Task LoginUserAsync(string userName, string password, Session session)
        {
            var user=await _dbContext.Users.SingleOrDefaultAsync(x=>x.UserName==userName);
            if(user==null)
                throw new MiniCloudException("Invalid Credentials");
            var hashedPassword=_encryptService.Compute(password,user.Salt);
            if(user.HashedPassword!=hashedPassword)
                throw new MiniCloudException("Invalid Credentials");
            session.AddObject("logged_user_id",user.Id);
        }
        public Task<User> GetLoggedUser(Session session)
        {
            if (!session.TryGetValue("logged_user_id", out object loggedUserIdObject))
                throw new MiniCloudException("No one is logged");
            var user=_dbContext.Users.SingleOrDefault(x=>x.Id==(int)loggedUserIdObject);
            return Task.FromResult(user);
        }
    }
}
