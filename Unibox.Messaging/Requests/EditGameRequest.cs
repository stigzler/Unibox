using NetMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messaging.DTOs;
using Unibox.Messaging.Responses;

namespace Unibox.Messaging.Requests
{
    public class EditGameRequest : IRequest<EditGameResponse>
    {
        public GameDTO Game { get; set; }
    }
}