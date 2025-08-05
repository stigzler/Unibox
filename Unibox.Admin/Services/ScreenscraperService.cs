using stigzler.ScreenscraperWrapper;
using stigzler.ScreenscraperWrapper.DTOs;
using stigzler.ScreenscraperWrapper.Results;
using stigzler.ScreenscraperWrapper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Admin.Services
{
    internal class ScreenscraperService
    {
        public ApiCredentials SsCredentials { get; set; } = new ApiCredentials();

        private ApiGet ApiGet = new ApiGet();

        public ScreenscraperService()
        {
        }

        internal void UpdateCredentialsFromUserSettings()
        {
            SsCredentials.DeveloperSoftware = Properties.Settings.Default.ssApiName;
            SsCredentials.DeveloperID = Properties.Settings.Default.ssApiUsername;
            SsCredentials.DeveloperPassword = Properties.Settings.Default.ssApiPassword;
            SsCredentials.UserID = Properties.Settings.Default.SsUsername;
            SsCredentials.UserPassword = Properties.Settings.Default.SsPassword;
        }

        internal async Task<ApiGetDataOutcome> GetScreenscraperSystems()
        {
            return await Task.Run(() => ApiGet.GetList(stigzler.ScreenscraperWrapper.Data.Enums.ApiListRequest.SystemList));
        }

        internal async Task<ApiGetDataOutcome> GetScreenscraperGameMediaTypes()
        {
            return await Task.Run(() => ApiGet.GetList(stigzler.ScreenscraperWrapper.Data.Enums.ApiListRequest.GameMediaTypeList));
        }
    }
}