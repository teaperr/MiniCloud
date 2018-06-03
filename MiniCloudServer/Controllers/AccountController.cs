using MiniCloud.Core;
using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MultiServer.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class AccountController: Controller
    {
        private readonly AccountService _accountService;
        public AccountController(Session session): base(session)
        {
            var dbContext = new MiniCloudContext();
            var encryptService = new EncryptService();
            _accountService = new AccountService(dbContext, encryptService, session);
        }
        private async Task<string> SayMyName()
        {
            var user = await _accountService.GetLoggedUser();
            return $"You are {user.UserName}";
        }
        private async Task<string> Login(string userName, string password)
        {
            //"Usage: login <user_name> <password>"
            await _accountService.LoginUserAsync(userName, password);
            return $"Welcome {userName}";
        }
        private async Task<string> Register(string userName, string password)
        {
            //Usage: register<user_name> < password >
            await _accountService.RegisterUserAsync(userName, password);
            return "Created.";
        }
    }
}
