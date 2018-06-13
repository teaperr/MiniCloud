using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services;
using MultiServer.Services;
using Server.Services;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public class AccountController: Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(Session session): base(session)
        {
            var dbContext = new MiniCloudContext();
            var encryptService = new EncryptService();
            _accountService = new AccountService(dbContext, encryptService, session);
        }
        public async Task<string> SayMyName()
        {
            var user = await _accountService.GetLoggedUser();
            return $"You are {user.UserName}";
        }
        public async Task<string> Login(string userName, string password)
        {
            await _accountService.LoginUserAsync(userName, password);
            return $"Welcome {userName}";
        }
        public async Task<string> Register(string userName, string password)
        {
            await _accountService.RegisterUserAsync(userName, password);
            return "Created.";
        }
    }
}
