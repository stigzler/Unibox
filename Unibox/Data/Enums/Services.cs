using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Enums
{

    internal enum UpdatePlatformMessageType
    {
        Success,
        Warning,
        Error,
        Information,
    }

    internal enum UpdatePlatformErrors
    {
        InstallationDirectoryDoesntExist,

    }

    internal enum UpdatePlatformOutcome
    {
        Indeterminate,
        CannotAccessInstallationDirectory,
        XmlFileDoesNotExist,
    }
}

