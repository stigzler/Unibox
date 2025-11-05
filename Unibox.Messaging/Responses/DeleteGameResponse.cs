using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Messaging.Responses
{
    public class DeleteGameResponse
    {
        public bool IsSuccessful { get; set; } = false;
        public string TextResult { get; set; } = string.Empty;
    }
}