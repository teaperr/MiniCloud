using SimpleCrypto;

namespace Server.Services
{
    public class EncryptService : IEncryptService
    {
        private readonly ICryptoService _simpleCrypto;

        public EncryptService()
        {
            _simpleCrypto = new PBKDF2();
        }

        public string GenerateSalt()
        {
            return _simpleCrypto.GenerateSalt();
        }
        public string Compute(string textToHash, string salt)
        {
            return _simpleCrypto.Compute(textToHash, salt);
        }
        public bool Compare(string hashedPassword1, string hashedPassword2)
        {
            return _simpleCrypto.Compare(hashedPassword1, hashedPassword2);
        }
    }
}
