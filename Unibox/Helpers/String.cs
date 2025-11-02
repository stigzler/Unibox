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

        public static bool ValidUrl(string url)
        {
            var urlRegex = new Regex(
                @"^(https?|ftps?):\/\/(?:[a-zA-Z0-9]" +
                        @"(?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}" +
                        @"(?::(?:0|[1-9]\d{0,3}|[1-5]\d{4}|6[0-4]\d{3}" +
                        @"|65[0-4]\d{2}|655[0-2]\d|6553[0-5]))?" +
                        @"(?:\/(?:[-a-zA-Z0-9@%_\+.~#?&=]+\/?)*)?$",
                RegexOptions.IgnoreCase);

            urlRegex.Matches(url);

            return urlRegex.IsMatch(url);
        }
    }
}