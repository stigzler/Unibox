using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Enums
{
    public enum UpdatePlatformMessageType
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

    public enum UpdatePlatformOutcome
    {
        Success,
        Indeterminate,
        CannotAccessInstallationDirectory,
        XmlFileDoesNotExist,
    }
}