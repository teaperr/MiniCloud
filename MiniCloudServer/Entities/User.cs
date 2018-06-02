using Server.Services;

namespace MiniCloud.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string HashedPassword { get; private set; }
        public string Salt { get; private set; }

        public User(string userName, string password)
        {
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
