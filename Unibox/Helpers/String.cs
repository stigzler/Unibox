using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Unibox.Helpers
{
    internal class String
    {
        public static string RemoveTags(string romName)
        {
            // Define a regex pattern to match tags at the end of the ROM name
            string pattern = @"\s*\(.*?\)|\s*\[.*?\]";

            // Remove the file extension
            string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(romName);

            // Use regex to replace the tags with an empty string
            string cleanedName = Regex.Replace(nameWithoutExtension, pattern, "").Trim();

            return cleanedName;
        }

        public static bool IsValidIPv4Address(string ipString)
        {
            if (System.String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}