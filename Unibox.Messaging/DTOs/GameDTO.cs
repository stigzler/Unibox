using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;

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

        public GameDTO(IGame game)
        {
            LaunchboxID = game.Id.ToString();
            Title = game.Title;
            Platform = game.Platform ?? string.Empty;
            Notes = game.Notes;
            Publisher = game.Publisher;
            Developer = game.Developer;
            ApplicationPath = game.ApplicationPath;
            ReleaseDate = game.ReleaseDate;
            DateAdded = game.DateAdded;
        }
    }
}