using NetMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Messaging.DTOs;
using Unibox.Messaging.Responses;

namespace Unibox.Messaging.Requests
{
    public class AddGameRequest : IRequest<AddGameResponse>
    {
        public GameDTO Game { get; set; }
    }
}