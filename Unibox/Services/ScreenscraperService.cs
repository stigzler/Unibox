using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            if (Properties.Settings.Default.SsApiName != null || Properties.Settings.Default.SsApiPassword != null ||
                Properties.Settings.Default.SsApiUsername != null)
            {
                SsCredentials.DeveloperSoftware = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiName);
                SsCredentials.DeveloperID = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiUsername);
                SsCredentials.DeveloperPassword = Helpers.Encryption.DpapiEncrypt.QuickDecrypt(Properties.Settings.Default.SsApiPassword);
            }
        }
    }
}