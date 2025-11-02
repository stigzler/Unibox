using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Messaging.DTOs
{
    public class GameDTO
    {
        public string LaunchboxID { get; set; } = string.Empty;
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Notes { get; set; }
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public string ApplicationPath { get; set; }
        public string EmulatorID { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? DateAdded { get; set; }

        public GameDTO(string title)
        {
            Title = title;
        }

        public GameDTO()
        {
        }
    }
}