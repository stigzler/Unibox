using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Messaging.DTOs;

namespace Unibox.Messaging.Responses
{
    public class GetCurrentGameResponse
    {
        public GameDTO Game { get; set; }
    }
}