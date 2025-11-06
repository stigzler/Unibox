using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Enums
{
    public enum DialogResult
    {
        Unset,
        OK,
        Cancel,
        Yes,
        No,
        UsePath
    }

    public enum PageRequestType
    {
        None,
        Installations,
        EditInstallation,
        InstallationPlatforms,
        AddGameResults,
        Games,
        PleaseWait,
        UpdatePlatformsResults,
        EditPlatform,
        EditGame,
        EditGameNotes
    }
}