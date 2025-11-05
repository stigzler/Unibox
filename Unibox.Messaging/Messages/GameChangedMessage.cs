using NetMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;

namespace Unibox.Messaging.Messages
{
    public class GameChangedMessage : MessageBase
    {
        public IGame Game { get; set; }

        public GameChangedMessage(IGame game)
        {
            MessageType = Enums.MessageType.GameChanged;
            Game = game;
        }
    }
}