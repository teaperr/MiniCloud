using MiniCloudServer.Core;
using MultiServer.Services;
using System.Threading.Tasks;

namespace MiniCloudServer.Controllers
{
    public abstract class Controller: IController
    {
        protected Session Session;
        protected readonly IAccountService AccountService;

        public Controller(IAccountService accountService)
        {
            AccountService = accountService;
        }

        public void SetSession(Session session)
        {
            Session=session;
        }
        public async Task<string> GetLoggedUserName()
        {
            var userName = (await AccountService.GetLoggedUser(Session)).UserName;
            return userName;
        }
    }
}
