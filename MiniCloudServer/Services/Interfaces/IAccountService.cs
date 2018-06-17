using System.Threading.Tasks;
using MiniCloudServer.Core;
using MiniCloudServer.Entities;
using MiniCloudServer.Services.Interfaces;

namespace MultiServer.Services
{
    public interface IAccountService:IService
    {
        Task<User> GetLoggedUser(Session session);
        Task LoginUserAsync(string userName, string password, Session session);
        Task RegisterUserAsync(string userName, string password);
    }
}