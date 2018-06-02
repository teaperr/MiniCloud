using Microsoft.EntityFrameworkCore;
using MiniCloud.Core;
using MiniCloud.Entities;
using MiniCloudServer.Exceptions;
using MiniCloudServer.Persistence;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServer.Services
{
    public class AccountService
    {
        private readonly MiniCloudContext _dbContext;
        private readonly IEncryptService _encryptService;
        private readonly Connection _connection;

        public AccountService(MiniCloudContext dbContext, IEncryptService encryptService, Connection connection)
        {
            _dbContext = dbContext;
            _encryptService=encryptService;
            _connection = connection;
        }
        public void RegisterUser(string userName, string password)
        {
            var user=_dbContext.Users.SingleOrDefault(x=>x.UserName==userName);
            if(user!=null)
                throw new MiniCloudException("User with this name already exists");
            user=new User(userName,password);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public void LoginUser(string userName, string password)
        {
            var user=_dbContext.Users.SingleOrDefault(x=>x.UserName==userName);
            if(user==null)
                throw new MiniCloudException("Invalid Credentials");
            var hashedPassword=_encryptService.Compute(password,user.Salt);
            if(user.HashedPassword!=hashedPassword)
                throw new MiniCloudException("Invalid Credentials");
            _connection.Session.AddObject("logged_user_id",user.Id);
        }
        public User GetLoggedUser()
        {
            object loggedUserIdObject;
            if(!_connection.Session.TryGetValue("logged_user_id",out loggedUserIdObject))
                throw new MiniCloudException("No one is logged");
            var user=_dbContext.Users.SingleOrDefault(x=>x.Id==(int)loggedUserIdObject);
            return user;
        }
    }
}
