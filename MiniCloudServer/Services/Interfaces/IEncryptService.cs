using MiniCloudServer.Services.Interfaces;

namespace Server.Services
{
    public interface IEncryptService : IService
    {
        bool Compare(string hashedPassword1, string hashedPassword2);
        string Compute(string textToHash, string salt);
        string GenerateSalt();
    }
}