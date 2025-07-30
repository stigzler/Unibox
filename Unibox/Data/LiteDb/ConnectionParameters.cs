using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.LiteDb
{
    public class ConnectionParameters
    {
        public ConnectionType ConnectionType { get; set; } = ConnectionType.Shared;
        public string Filename { get; set; } = null;
        public string InitialSize { get; set; } = null;
        public string Password { get; set; } = null;
        public bool? ReadOnly { get; set; } = null;
        public bool? Upgrade { get; set; } = null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("\"");
            if (Filename != null) sb.Append("Filename=" + Filename + ";");
            sb.Append("Connection=" + ConnectionType.ToString() + ";");
            if (Password != null) sb.Append("Password=" + Password + ";");
            if (InitialSize != null) sb.Append("InitialSize=" + InitialSize + ";");
            if (ReadOnly != null) sb.Append("ReadOnly=" + ReadOnly + ";");
            if (Upgrade != null) sb.Append("Upgrade=" + Upgrade.ToString().ToLower() + ";");

            return sb.ToString().Remove(sb.Length - 1); // removes trailing ";"
        }
    }
}
