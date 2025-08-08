using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Helpers.Encryption
{
    internal class DpapiEncrypt
    {
        // Encrypts using user hardware
        // Exmaple usage:
        //public string PersonalAccessToken
        //{
        //get => Encryption.DpapiToInsecureString(Encryption.DpapiDecryptString(personalAccessToken));
        //set => personalAccessToken = Encryption.DpapiEncryptString(Encryption.DpapiToSecureString(value));
        //}

        private static byte[] entropy = Encoding.Unicode.GetBytes("vYowIchyZq11qDTvpm6KJ1yq1MFlIU4j");

        public static string QuickDecrypt(string encryptedString)
        {
            return DpapiToInsecureString(DpapiDecryptString(encryptedString));
        }

        public static string QuickEncrypt(string unencryptedString)
        {
            return DpapiEncryptString(DpapiToSecureString(unencryptedString));
        }

        public static string DpapiEncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(DpapiToInsecureString(input)),
                entropy,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DpapiDecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    DataProtectionScope.CurrentUser);
                return DpapiToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString DpapiToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string DpapiToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
}