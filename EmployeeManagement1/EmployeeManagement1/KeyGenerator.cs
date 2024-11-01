using System.Security.Cryptography;

namespace EmployeeManagement
{
    public static class KeyGenerator
    {
        public static string GenerateBase64Key()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var key = new byte[32]; // 32 bytes = 256 bits
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }
    }
}
