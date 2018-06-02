namespace Server.Services
{
    public interface IEncryptService
    {
        bool Compare(string hashedPassword1, string hashedPassword2);
        string Compute(string textToHash, string salt);
        string GenerateSalt();
    }
}