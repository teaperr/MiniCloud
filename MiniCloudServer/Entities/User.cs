using MiniCloudServer.Persistence;
using Server.Services;
using System;
using System.Collections.Generic;

namespace MiniCloudServer.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string HashedPassword { get; private set; }
        public string Salt { get; private set; }

        public ICollection<ResourceAccess> ResourceAccesses {get; private set; }

        public User(string userName, string password)
        {
            ResourceAccesses=new HashSet<ResourceAccess>();
            SetUserName(userName);
            SetPassword(password);
        }
        private User()
        {

        }

        public void SetUserName(string userName)
        {
            UserName=userName;
        }
        public void SetPassword(string password)
        {
            var encrypterService = new EncryptService();
            Salt = encrypterService.GenerateSalt();
            HashedPassword = encrypterService.Compute(password, Salt);
        }
        
    }
}
