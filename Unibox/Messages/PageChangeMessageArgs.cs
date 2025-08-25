using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Enums;

namespace Unibox.Messages
{
    internal class PageChangeMessageArgs
    {
        public PageRequestType RequestType { get; set; } = PageRequestType.None;
        public object? Data { get; set; } = null;
    }
}