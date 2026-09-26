using System.Security.Cryptography;
using System.Text;

namespace AssignmentTracker.Data
{
    public static class PasswordHelper
    {
        public static string Hash(string plainPassword)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(plainPassword);
            byte[] hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool Verify(string plainPassword, string hashedPassword)
        {
            return Hash(plainPassword) == hashedPassword;
        }
    }
}
