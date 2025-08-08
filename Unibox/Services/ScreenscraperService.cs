using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Unibox.Services
{
    public class ScreenscraperService
    {
        private ApiCredentials SsCredentials { get; set; } = new ApiCredentials();
        private ApiGet ApiGet { get; set; } = new ApiGet();

        public ScreenscraperService()
        {
        }

        internal void UpdateCredentialsFromUserSettings()
        {
            SsCredentials.UserID = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsUsername);
            SsCredentials.UserPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsPassword);

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SsApiName)
                || !String.IsNullOrWhiteSpace(Properties.Settings.Default.SsApiPassword)
                || !String.IsNullOrWhiteSpace(Properties.Settings.Default.SsApiUsername))
            {
                SsCredentials.DeveloperSoftware = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiName);
                SsCredentials.DeveloperID = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiUsername);
                SsCredentials.DeveloperPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiPassword);
            }
            else
            {
                // Use API credentials from the Resources\Files\secrets.txt file (not included in the repo - see the repo README for details)
                string secrets = Helpers.FileSystem.ReadEmbeddedResourceFile("secrets.txt");
                string[] lines = secrets.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string encryptionKey = lines[0];
                SsCredentials.DeveloperSoftware = Helpers.Encryption.AESEncrypt.Decrypt(lines[1], encryptionKey);
                SsCredentials.DeveloperID = Helpers.Encryption.AESEncrypt.Decrypt(lines[2], encryptionKey);
                SsCredentials.DeveloperPassword = Helpers.Encryption.AESEncrypt.Decrypt(lines[3], encryptionKey);
            }
        }
    }
}