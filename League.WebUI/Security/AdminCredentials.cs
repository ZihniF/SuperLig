using Microsoft.AspNetCore.Identity;

namespace League.WebUI.Security
{
    public class AdminCredentials
    {
        private readonly PasswordHasher<string> _hasher = new();
        private readonly string _hash;

        public string Username { get; }

        public AdminCredentials(string username, string password)
        {
            Username = username;
            _hash = _hasher.HashPassword(username, password);
        }

        public bool Verify(string username, string password)
        {
            var verified = _hasher.VerifyHashedPassword(Username, _hash, password)
                != PasswordVerificationResult.Failed;

            return verified && string.Equals(username, Username, StringComparison.Ordinal);
        }
    }
}
