using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Services;
using stigzler.ScreenscraperWrapper.Data.Entities.Screenscraper;
using stigzler.ScreenscraperWrapper.Data.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using stigzler.ScreenscraperWrapper.Results;

namespace Unibox.Services
{
    public class ScreenscraperService
    {
        private ApiCredentials SsCredentials { get; set; } = new ApiCredentials();

        private ApiGet ApiGet { get; set; } = new ApiGet();

        public ScreenscraperService(FileService fileService)
        {
            UpdateCredentialsFromUserSettings();
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
                // Use API credentials from the Resources\Files\info.txt file (not included in the repo - see the repo README for details)
                string secrets = Helpers.FileSystem.ReadEmbeddedResourceFile("info.txt");
                string[] lines = secrets.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string encryptionKey = lines[0];
                SsCredentials.DeveloperSoftware = Helpers.Encryption.AESEncrypt.Decrypt(lines[1], encryptionKey);
                SsCredentials.DeveloperID = Helpers.Encryption.AESEncrypt.Decrypt(lines[2], encryptionKey);
                SsCredentials.DeveloperPassword = Helpers.Encryption.AESEncrypt.Decrypt(lines[3], encryptionKey);
            }

            ApiGet = new ApiGet(SsCredentials, new ApiServerParameters(), MetadataOutputFormat.xml);
            ApiGet.UserThreads = Properties.Settings.Default.ssThreads;
        }

        internal async Task<ApiGetDataOutcome> GetUser()
        {
            return await ApiGet.GetList(ApiListRequest.UserInfo);
        }

        internal async Task<ApiGetDataOutcome> GetGameByRomName(string romName, int systemID)
        {
            return await ApiGet.GetGame(ApiSearchRequest.GameRomSearch,
                new ApiGameSearchParameters()
                {
                    RomName = romName,
                    SystemID = systemID
                });
        }

        internal async Task<List<ApiGetFileOutcome>> GetMediaFiles(List<ApiFileDownloadParameters> apiFileDownloadParameters)
        {
            return await ApiGet.GetFiles(apiFileDownloadParameters);
        }
    }
}