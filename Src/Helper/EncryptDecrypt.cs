using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace BitDesk.Helper;

internal static class EncryptDecrypt
{
    internal static string Decrypt(string secret, string key, string salt)
    {
        if (string.IsNullOrEmpty(secret)) { return ""; }
        if (string.IsNullOrEmpty(key)) { return ""; }

        // Uses System.Security.Cryptography.ProtectedData
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //byte[] entropy = [0x72, 0xa2, 0x12, 0x04];
            byte[] entropy = Encoding.UTF8.GetBytes(key);

            try
            {
                byte[] encryptedData = System.Convert.FromBase64String(secret);
                byte[] userData = System.Security.Cryptography.ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);

                return System.Text.Encoding.UTF8.GetString(userData);
            }
            catch
            {
                Debug.WriteLine($"Decrypt fail.");
                return "";
            }
        }
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        else
        {
            try
            {
                byte[] encryptionKey = Encoding.UTF8.GetBytes(key);
                byte[] entropy = Encoding.UTF8.GetBytes(salt);

                secret = secret.Replace(" ", "+");
                byte[] sBytes = Convert.FromBase64String(secret);
                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    // Obsolete and deprecated in .NET 10.
                    //Rfc2898DeriveBytes pdb = new(encryptionKey, entropy, 10000, HashAlgorithmName.SHA1);
                    //encryptor.Key = pdb.GetBytes(32);
                    //encryptor.IV = pdb.GetBytes(16);
                    // New since .NET 10.
                    encryptor.Key = Rfc2898DeriveBytes.Pbkdf2(key, entropy, 10000, HashAlgorithmName.SHA1, 32);
                    encryptor.IV = Rfc2898DeriveBytes.Pbkdf2(key, entropy, 10000, HashAlgorithmName.SHA1, 16);

                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(sBytes, 0, sBytes.Length);
                        cs.Close();
                    }
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            catch
            {
                Debug.WriteLine($"Decrypt fail.");
                return "";
            }
        }
    }

    internal static string Encrypt(string secret, string key, string salt)
    {
        if (string.IsNullOrEmpty(secret)) { return ""; }
        if (string.IsNullOrEmpty(key)) { return ""; }


        // Uses System.Security.Cryptography.ProtectedData
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //byte[] entropy = [0x72, 0xa2, 0x12, 0x04];
            byte[] entropy = Encoding.UTF8.GetBytes(key);

            try
            {
                byte[] userData = System.Text.Encoding.UTF8.GetBytes(secret);
                byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(userData, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);

                return System.Convert.ToBase64String(encryptedData);
            }
            catch
            {
                Debug.WriteLine("Encrypt fail.");
                return "";
            }
        }
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        else
        {
            try
            {
                byte[] encryptionKey = Encoding.UTF8.GetBytes(key);
                byte[] entropy = Encoding.UTF8.GetBytes(salt);

                byte[] sBytes = Encoding.Unicode.GetBytes(secret);
                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    // Obsolete and deprecated in .NET 10.
                    //Rfc2898DeriveBytes pdb = new(encryptionKey, entropy, 10000, HashAlgorithmName.SHA1);
                    //encryptor.Key = pdb.GetBytes(32);
                    //encryptor.IV = pdb.GetBytes(16);
                    // New since .NET 10.
                    encryptor.Key = Rfc2898DeriveBytes.Pbkdf2(key, entropy, 10000, HashAlgorithmName.SHA1, 32);
                    encryptor.IV = Rfc2898DeriveBytes.Pbkdf2(key, entropy, 10000, HashAlgorithmName.SHA1, 16);

                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(sBytes, 0, sBytes.Length);
                        cs.Close();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch
            {
                Debug.WriteLine($"Encrypt fail.");
                return "";
            }
        }
    }

}

