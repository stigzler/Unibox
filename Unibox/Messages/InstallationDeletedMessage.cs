using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;

namespace Unibox.Messages
{
    class InstallationDeletedMessage : ValueChangedMessage<InstallationModel>
    {
        public InstallationDeletedMessage(InstallationModel value) : base(value)
        {
        }
    }
}
