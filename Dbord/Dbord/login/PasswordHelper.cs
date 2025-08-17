using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    // Generate a random salt
    public static string GenerateSalt(int size = 16)
    {
        var rng = new RNGCryptoServiceProvider();
        byte[] saltBytes = new byte[size];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    // Hash password with salt
    public static string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var combined = password + salt;
            byte[] bytes = Encoding.UTF8.GetBytes(combined);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
