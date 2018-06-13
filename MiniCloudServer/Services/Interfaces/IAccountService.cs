using System.Threading.Tasks;
using MiniCloud.Entities;

namespace MultiServer.Services
{
    public interface IAccountService
    {
        Task<User> GetLoggedUser();
        Task LoginUserAsync(string userName, string password);
        Task RegisterUserAsync(string userName, string password);
    }
}