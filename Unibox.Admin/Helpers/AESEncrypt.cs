using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Admin.Helpers
{
    internal class AESEncrypt
    {
        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    // Replace the following line:
                    // var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    // With the updated constructor specifying the hash algorithm:
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA256);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA256);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static string Encrypt(string text, string pwd)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwd);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Generating salt bytes
            byte[] saltBytes = GetRandomBytes();

            // Appending salt bytes to original bytes
            byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
            for (int i = 0; i < saltBytes.Length; i++)
            {
                bytesToBeEncrypted[i] = saltBytes[i];
            }
            for (int i = 0; i < originalBytes.Length; i++)
            {
                bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
            }

            encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string decryptedText, string pwd)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwd);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            // Getting the size of salt
            int _saltSize = 4;

            // Removing salt bytes, retrieving original bytes
            byte[] originalBytes = new byte[decryptedBytes.Length - _saltSize];
            for (int i = _saltSize; i < decryptedBytes.Length; i++)
            {
                originalBytes[i - _saltSize] = decryptedBytes[i];
            }

            return Encoding.UTF8.GetString(originalBytes);
        }

        public static byte[] GetRandomBytes()
        {
            int _saltSize = 4;
            byte[] ba = new byte[_saltSize];
            System.Security.Cryptography.RandomNumberGenerator.Fill(ba);
            return ba;
        }
    }
}