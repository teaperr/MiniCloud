using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using MultiServer.Services;
using Server.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class AccountController: Controller
    {

        public AccountController(IAccountService accountService): base(accountService)
        {
        }

        public async Task<string> SayMyName()
        {
            var user = await AccountService.GetLoggedUser(Session);
            return $"You are {user.UserName}";
        }
        public async Task<string> Login(string userName, string password)
        {
            await AccountService.LoginUserAsync(userName, password, Session);
            return $"Welcome {userName}";
        }
        public async Task<string> Register(string userName, string password)
        {
            await AccountService.RegisterUserAsync(userName, password);
            return "Created.";
        }
    }
}
