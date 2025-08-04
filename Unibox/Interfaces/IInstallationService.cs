using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Interfaces
{
    internal interface IInstallationService
    {
        InstallationModel AddNew();
        void Update(InstallationModel installationModel);
    }
}
