using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BitDesk.Helper;

public class Password
{
    private const int SaltSize = 16;         // 128-bit salt
    private const int HashSize = 32;         // 256-bit hash
    private const int Iterations = 600000;   // Industry standard speed bump
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public static (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            passwordBytes,
            salt,
            Iterations,
            HashAlgorithm,
            HashSize
        );

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(string enteredPassword, string storedHashStr, string storedSaltStr)
    {
        byte[] storedHash = Convert.FromBase64String(storedHashStr);
        byte[] storedSalt = Convert.FromBase64String(storedSaltStr);

        byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
            enteredPasswordBytes,
            storedSalt,
            Iterations,
            HashAlgorithm,
            HashSize
        );

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}